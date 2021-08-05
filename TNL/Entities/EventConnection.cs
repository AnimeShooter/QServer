using System;

namespace TNL.Entities
{
    using Data;
    using Notify;
    using Types;
    using Utils;

    public class EventNote
    {
        public NetEvent Event { get; set; }
        public int SeqCount { get; set; }
        public EventNote NextEvent { get; set; }
    }

    public class EventConnection : NetConnection
    {
        public const uint DebugCheckSum = 0xF00DBAADU;
        public const byte BitStreamPosBitSize = 16;
        public const int InvalidSendEventSeq = -1;
        public const int FirstValidSendEventSeq = 0;

        private EventNote _sendEventQueueHead;
        private EventNote _sendEventQueueTail;
        private EventNote _unorderedSendEventQueueHead;
        private EventNote _unorderedSendEventQueueTail;
        private EventNote _waitSeqEvents;
        private EventNote _notifyEventList;

        private int _nextSendEventSeq;
        private int _nextRecvEventSeq;
        private int _lastAckedEventSeq;
        private readonly float _packetFillFraction;

        protected uint EventClassCount;
        protected uint EventClassBitSize;
        protected uint EventClassVersion;

        public uint NumEventsWaiting { get; set; }

        public EventConnection()
        {
            _notifyEventList = null;
            _sendEventQueueHead = null;
            _sendEventQueueTail = null;
            _unorderedSendEventQueueHead = null;
            _unorderedSendEventQueueTail = null;
            _waitSeqEvents = null;

            _nextSendEventSeq = FirstValidSendEventSeq;
            _nextRecvEventSeq = FirstValidSendEventSeq;
            _lastAckedEventSeq = -1;

            EventClassCount = 0;
            EventClassBitSize = 0;

            NumEventsWaiting = 0;

            _packetFillFraction = 1.0f;
        }

        ~EventConnection()
        {
            while (_notifyEventList != null)
            {
                var temp = _notifyEventList;
                _notifyEventList = temp.NextEvent;

                temp.Event.NotifyDelivered(this, true);
            }

            while (_unorderedSendEventQueueHead != null)
            {
                var temp = _unorderedSendEventQueueHead;
                _unorderedSendEventQueueHead = temp.NextEvent;

                temp.Event.NotifyDelivered(this, true);
            }

            while (_sendEventQueueHead != null)
            {
                var temp = _sendEventQueueHead;
                _sendEventQueueHead = temp.NextEvent;

                temp.Event.NotifyDelivered(this, true);
            }
        }

        protected override PacketNotify AllocNotify()
        {
            return new EventPacketNotify();
        }

        protected override void PacketDropped(PacketNotify note)
        {
            base.PacketDropped(note);

            if (note is not EventPacketNotify notify)
                throw new ArgumentException("Note must be EventPacketNotify", nameof(note));

            var walk = notify.EventList;
            var insertList = _sendEventQueueHead;
            var insertListPrev = _sendEventQueueHead;

            while (walk != null)
            {
                var temp = walk.NextEvent;

                switch (walk.Event.GuaranteeType)
                {
                    case GuaranteeType.GuaranteedOrdered:
                        while (insertList != null && insertList.SeqCount < walk.SeqCount)
                        {
                            insertListPrev = insertList;
                            insertList = insertList.NextEvent;
                        }

                        walk.NextEvent = insertList;

                        if (walk.NextEvent == null)
                            _sendEventQueueTail = walk;

                        if (insertListPrev == null)
                            _sendEventQueueHead = walk;
                        else
                            insertListPrev.NextEvent = walk;

                        insertListPrev = walk;
                        insertList = walk.NextEvent;
                        break;

                    case GuaranteeType.Guaranteed:
                        walk.NextEvent = _unorderedSendEventQueueHead;
                        _unorderedSendEventQueueHead = walk;

                        if (walk.NextEvent == null)
                            _unorderedSendEventQueueTail = walk;

                        break;

                    case GuaranteeType.Unguaranteed:
                        walk.Event.NotifyDelivered(this, false);
                        break;
                }

                walk = temp;
                ++NumEventsWaiting;
            }
        }

        protected override void PacketReceived(PacketNotify note)
        {
            base.PacketReceived(note);

            if (note is not EventPacketNotify notify)
                throw new ArgumentException("Note must be EventPacketNotify", nameof(note));

            var walk = notify.EventList;
            var noteList = _notifyEventList;
            var noteListPrev = _notifyEventList;

            while (walk != null)
            {
                var next = walk.NextEvent;

                if (walk.Event.GuaranteeType != GuaranteeType.GuaranteedOrdered)
                    walk.Event.NotifyDelivered(this, true);
                else
                {
                    while (noteList != null && noteList.SeqCount < walk.SeqCount)
                    {
                        noteListPrev = noteList;
                        noteList = noteList.NextEvent;
                    }

                    walk.NextEvent = noteList;

                    if (noteListPrev == null)
                        _notifyEventList = walk;
                    else
                        noteListPrev.NextEvent = walk;

                    noteListPrev = walk;
                    noteList = walk.NextEvent;
                }

                walk = next;
            }

            while (_notifyEventList != null && _notifyEventList.SeqCount == _lastAckedEventSeq + 1)
            {
                ++_lastAckedEventSeq;
                var next = _notifyEventList.NextEvent;

                _notifyEventList.Event.NotifyDelivered(this, true);

                _notifyEventList = next;
            }
        }

        protected override void WritePacket(BitStream stream, PacketNotify note)
        {
            base.WritePacket(stream, note);

            if (note is not EventPacketNotify notify)
                throw new ArgumentException("Note must be EventPacketNotify", nameof(note));

            if (ConnectionParameters.DebugObjectSizes)
                stream.WriteInt(DebugCheckSum, 32);

            EventNote packQueueHead = null, packQueueTail = null;

            var totalPacketSpaceFraction = 1.0f / stream.MaxWriteBitNum;

            while (_unorderedSendEventQueueHead != null)
            {
                if (stream.IsFull() || (stream.GetBitPosition() * totalPacketSpaceFraction) > _packetFillFraction)
                    break;

                var ev = _unorderedSendEventQueueHead;
                stream.WriteFlag(true);

                var start = stream.GetBitPosition();

                if (ConnectionParameters.DebugObjectSizes)
                    stream.AdvanceBitPosition(BitStreamPosBitSize);

                var classId = ev.Event.GetClassId(GetNetClassGroup());
                stream.WriteInt(classId, (byte) EventClassBitSize);

                ev.Event.Pack(this, stream);

                if (ConnectionParameters.DebugObjectSizes)
                    stream.WriteIntAt(stream.GetBitPosition(), BitStreamPosBitSize, start);

                if (stream.GetBitSpaceAvailable() < MinimumPaddingBits)
                {
                    stream.SetBitPosition(start - 1);
                    stream.ClearError();
                    break;
                }

                --NumEventsWaiting;

                _unorderedSendEventQueueHead = ev.NextEvent;
                ev.NextEvent = null;

                if (packQueueHead == null)
                    packQueueHead = ev;
                else
                    packQueueTail.NextEvent = ev;

                packQueueTail = ev;
            }

            stream.WriteFlag(false);
            var prevSeq = -2;

            while (_sendEventQueueHead != null)
            {
                if (stream.IsFull())
                    break;

                if (_sendEventQueueHead.SeqCount > _lastAckedEventSeq + 126)
                    break;

                var ev = _sendEventQueueHead;
                var eventStart = stream.GetBitPosition();

                stream.WriteFlag(true);

                if (!stream.WriteFlag(ev.SeqCount == prevSeq + 1))
                    stream.WriteInt((uint) ev.SeqCount, 7);

                prevSeq = ev.SeqCount;

                if (ConnectionParameters.DebugObjectSizes)
                    stream.AdvanceBitPosition(BitStreamPosBitSize);

                var start = stream.GetBitPosition();

                var classId = ev.Event.GetClassId(GetNetClassGroup());

                stream.WriteInt(classId, (byte) EventClassBitSize);

                ev.Event.Pack(this, stream);

                ev.Event.GetClassRep().AddInitialUpdate(stream.GetBitPosition() - start);

                if (ConnectionParameters.DebugObjectSizes)
                    stream.WriteIntAt(stream.GetBitPosition(), BitStreamPosBitSize, start);
                    //stream.WriteIntAt(stream.GetBitPosition(), BitStreamPosBitSize, start - BitStreamPosBitSize);

                if (stream.GetBitSpaceAvailable() < MinimumPaddingBits)
                {
                    stream.SetBitPosition(eventStart);
                    stream.ClearError();
                    break;
                }

                --NumEventsWaiting;

                _sendEventQueueHead = ev.NextEvent;
                ev.NextEvent = null;

                if (packQueueHead == null)
                    packQueueHead = ev;
                else
                    packQueueTail.NextEvent = ev;

                packQueueTail = ev;
            }

            for (var ev = packQueueHead; ev != null; ev = ev.NextEvent)
                ev.Event.NotifySent(this);

            notify.EventList = packQueueHead;
            stream.WriteFlag(false);
        }

        protected override void ReadPacket(BitStream stream)
        {
            base.ReadPacket(stream);

            if (ConnectionParameters.DebugObjectSizes)
                Console.WriteLine("{0:X8} == {1:X8}", stream.ReadInt(32), DebugCheckSum);

            var prevSeq = -2;
            var waitInsert = _waitSeqEvents;
            var waitInsertPrev = _waitSeqEvents;
            var ungaranteedPhase = true;

            while (true)
            {
                var bit = stream.ReadFlag();
                if (ungaranteedPhase && !bit)
                {
                    ungaranteedPhase = false;
                    bit = stream.ReadFlag();
                }

                if (!ungaranteedPhase && !bit)
                    break;

                var seq = -1;

                if (!ungaranteedPhase)
                {
                    if (stream.ReadFlag())
                        seq = (prevSeq + 1) & 0x7F;
                    else
                        seq = (int) stream.ReadInt(7);

                    prevSeq = seq;
                }

                var endingPosition = 0U;
                if (ConnectionParameters.DebugObjectSizes)
                    endingPosition = stream.ReadInt(BitStreamPosBitSize);

                var classId = stream.ReadInt((byte) EventClassBitSize);
                if (classId >= EventClassCount)
                {
                    SetLastError("Invalid packet.");
                    return;
                }

                var evt = (NetEvent) Create((uint) GetNetClassGroup(), (uint) NetClassType.NetClassTypeEvent, (int) classId);
                if (evt == null)
                {
                    SetLastError("Invalid packet.");
                    return;
                }

                if (evt.GetEventDirection() == EventDirection.DirUnset ||
                    (evt.GetEventDirection() == EventDirection.DirServerToClient && IsConnectionToClient()) ||
                    (evt.GetEventDirection() == EventDirection.DirClientToServer && IsConnectionToServer()))
                {
                    SetLastError("Invalid packet.");
                    return;
                }

                evt.Unpack(this, stream);
                if (ErrorBuffer[0] != 0)
                    return;

                if (ConnectionParameters.DebugObjectSizes)
                    Console.WriteLine("Assert({0:X8} == {1:X8}) || unpack did not match pack for event of class {2}.", endingPosition, stream.GetBitPosition(), evt.GetClassName());

                if (ungaranteedPhase)
                {
                    ProcessEvent(evt);

                    if (ErrorBuffer[0] != 0)
                        return;

                    continue;
                }

                seq |= (_nextRecvEventSeq & ~0x7F);
                if (seq < _nextRecvEventSeq)
                    seq += 128;

                var note = new EventNote
                {
                    Event = evt,
                    SeqCount = seq,
                    NextEvent = null
                };

                while (waitInsert != null && waitInsert.SeqCount < seq)
                {
                    waitInsertPrev = waitInsert;
                    waitInsert = waitInsert.NextEvent;
                }

                note.NextEvent = waitInsert;

                if (waitInsertPrev == null)
                    _waitSeqEvents = note;
                else
                    waitInsertPrev.NextEvent = note;

                waitInsertPrev = note;
                waitInsert = note.NextEvent;
            }

            while (_waitSeqEvents != null && _waitSeqEvents.SeqCount == _nextRecvEventSeq)
            {
                ++_nextRecvEventSeq;

                var temp = _waitSeqEvents;
                _waitSeqEvents = temp.NextEvent;

                ProcessEvent(temp.Event);

                if (ErrorBuffer[0] != 0)
                    return;
            }
        }

        public override bool IsDataToTransmit()
        {
            return _unorderedSendEventQueueHead != null || _sendEventQueueHead != null || base.IsDataToTransmit();
        }

        public void ProcessEvent(NetEvent theEvent)
        {
            if (GetConnectionState() == NetConnectionState.Connected)
                theEvent.Process(this);
        }

        public override void WriteConnectRequest(BitStream stream)
        {
            base.WriteConnectRequest(stream);

            stream.Write(NetClassRep.GetNetClassCount((uint) GetNetClassGroup(), (uint) NetClassType.NetClassTypeEvent));
        }

        public override bool ReadConnectRequest(BitStream stream, ref string errorString)
        {
            if (!base.ReadConnectRequest(stream, ref errorString))
                return false;

            stream.Read(out uint classCount);

            var myCount = NetClassRep.GetNetClassCount((uint) GetNetClassGroup(), (uint) NetClassType.NetClassTypeEvent);
            if (myCount <= classCount)
                EventClassCount = myCount;
            else
            {
                EventClassCount = classCount;
                if (!NetClassRep.IsVersionBorderCount((uint) GetNetClassGroup(), (uint) NetClassType.NetClassTypeEvent, EventClassVersion))
                    return false;
            }

            EventClassVersion = (uint) NetClassRep.GetClass((uint) GetNetClassGroup(), (uint) NetClassType.NetClassTypeEvent, EventClassCount - 1).ClassVersion;
            EventClassBitSize = Utils.GetNextBinLog2(EventClassCount);
            return true;
        }

        public override void WriteConnectAccept(BitStream stream)
        {
            base.WriteConnectAccept(stream);

            stream.Write(EventClassCount);
        }

        public override bool ReadConnectAccept(BitStream stream, ref string errorString)
        {
            if (!base.ReadConnectAccept(stream, ref errorString))
                return false;

            stream.Read(out EventClassCount);
            var myCount = NetClassRep.GetNetClassCount((uint) GetNetClassGroup(), (uint) NetClassType.NetClassTypeEvent);

            if (EventClassCount > myCount)
                return false;

            if (!NetClassRep.IsVersionBorderCount((uint) GetNetClassGroup(), (uint) NetClassType.NetClassTypeEvent, EventClassCount))
                return false;

            EventClassBitSize = Utils.GetNextBinLog2(EventClassCount);
            return true;
        }

        public uint GetEventClassVersion()
        {
            return EventClassVersion;
        }

        public bool PostNetEvent(NetEvent theEvent)
        {
            var classId = theEvent.GetClassId(GetNetClassGroup());
            if (classId >= EventClassCount && GetConnectionState() == NetConnectionState.Connected)
                return false;

            theEvent.NotifyPosted(this);

            var ev = new EventNote
            {
                Event = theEvent,
                NextEvent = null
            };

            if (ev.Event.GuaranteeType == GuaranteeType.GuaranteedOrdered)
            {
                ev.SeqCount = _nextSendEventSeq++;

                if (_sendEventQueueHead == null)
                    _sendEventQueueHead = ev;
                else
                    _sendEventQueueTail.NextEvent = ev;

                _sendEventQueueTail = ev;
            }
            else
            {
                ev.SeqCount = InvalidSendEventSeq;

                if (_unorderedSendEventQueueHead == null)
                    _unorderedSendEventQueueHead = ev;
                else
                    _unorderedSendEventQueueTail.NextEvent = ev;

                _unorderedSendEventQueueTail = ev;
            }

            ++NumEventsWaiting;
            return true;
        }
    }
}
