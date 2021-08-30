using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace TNL.Entities
{
    using Data;
    using Network;
    using Notify;
    using Structures;
    using Types;
    using Utils;

    public enum NetConnectionState
    {
        NotConnected = 0,
        AwaitingChallengeResponse,
        SendingPunchPackets,
        ComputingPuzzleSolution,
        AwaitingConnectResponse,
        ConnectTimedOut,
        ConnectRejected,
        Connected,
        Disconnected,
        TimedOut,
        StateCount
    };

    public enum NetPacketType
    {
        DataPacket        = 0,
        PingPacket        = 1,
        AckPacket         = 2,
        InvalidPacketType = 3
    }

    [Flags]
    public enum NetConnectionTypeFlags : uint
    {
        ConnectionToServer       = 1,
        ConnectionToClient       = 2,
        ConnectionAdaptive       = 4,
        ConnectionRemoteAdaptive = 8
    }

    public class NetConnection : BaseObject
    {
        #region Consts

        public const uint DefaultFixedBandwidth = 2500;
        public const uint DefaultFixedSendPeriod = 96;
        public const uint MaxFixedBandwidth = 65535;
        public const uint MaxFixedSendPeriod = 2047;
        public const uint MinimumPaddingBits = 128;
        public const uint MaxPacketDataSize = 1490;

        public const int MaxPacketWindowSizeShift = 5;
        public const int MaxPacketWindowSize = (1 << MaxPacketWindowSizeShift);
        public const int PacketWindowMask = MaxPacketWindowSize - 1;
        public const int MaxAckMaskSize = 1 << (MaxPacketWindowSizeShift - 5);
        public const int MaxAckByteCount = MaxAckMaskSize << 2;
        public const int SequenceNumberBitSize = 11;
        public const int SequenceNumberWindowSize = (1 << SequenceNumberBitSize);
        public const int SequenceNumberMask = -SequenceNumberWindowSize;
        public const int AckSequenceNumberBitSize = 10;
        public const int AckSequenceNumberWindowSize = (1 << AckSequenceNumberBitSize);
        public const int AckSequenceNumberMask = -AckSequenceNumberWindowSize;
        public const int PacketHeaderBitSize = 3 + AckSequenceNumberBitSize + SequenceNumberBitSize;
        public const int PacketHeaderByteSize = (PacketHeaderBitSize + 7) >> 3;
        public const int PacketHeaderPadBits = (PacketHeaderByteSize << 3) - PacketHeaderBitSize;
        public const int MessageSignatureBytes = 5;

        public const int AdaptiveInitialPingTimeout = 60000;
        public const int AdaptivePingRetryCount = 4;
        public const int DefaultPingTimeout = 5000;
        public const int DefaultPingRetryCount = 10;
        public const int AdaptiveUnackedSentPingTimeout = 3000;

        #endregion Consts

#pragma warning disable CA2211 // Non-constant fields should not be visible
        protected static char[] ErrorBuffer = new char[256];
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public static readonly string[] PacketTypeNames =
        {
            "DataPacket",
            "PingPacket",
            "AckPacket"
        };

        #region Properties

        public int LastPacketRecvTime { get; set; }

        protected uint[] LastSeqRecvdAtSend { get; set; }
        protected uint LastSeqRecvd { get; set; }
        protected uint HighestAckedSeq { get; set; }
        protected uint LastSendSeq { get; set; }
        protected uint[] AckMask { get; set; }

        private uint LastRecvAckAck { get; set; }
        private uint InitialSendSeq { get; set; }
        private uint InitialRecvSeq { get; set; }
        private int HighestAckedSendTime { get; set; }
        private int PingTimeout { get; set; }
        private int PingRetryCount { get; set; }

        public float RoundTripTime { get; set; }

        private BitSet TypeFlags { get; set; }
        private int LastUpdateTime { get; set; }
        private uint SendDelayCredit { get; set; }
        private int SimulatedLatency { get; set; }
        private float SimulatedPacketLoss { get; set; }
        private NetRate RemoteRate { get; set; }
        private bool LocalRateChanged { get; set; }
        protected uint CurrentPacketSendSize { get; set; } // Custom
        protected uint CurrentPacketSendPeriod { get; set; } // Custom
        private IPEndPoint NetAddress { get; set; }
        private int PingSendCount { get; set; }
        private int LastPingSendTime { get; set; }

        public int NumNotifies { get; set; }

        protected PacketNotify NotifyQueueHead { get; set; }
        protected PacketNotify NotifyQueueTail { get; set; }
        protected NetConnection RemoteConnection { get; set; }
        protected ConnectionParameters ConnectionParameters { get; set; }
        protected NetRate LocalRate { get; set; }

        public int ConnectSendCount { get; set; }
        public int ConnectLastSendTime { get; set; }

        protected NetInterface Interface { get; set; }
        protected SymmetricCipher SymmetricCipher { get; set; }

        public NetConnectionState ConnectionState { get; set; }

        private float Cwnd { get; set; }
        private float SSThresh { get; set; }
        private uint LastSeqRecvdAck { get; set; }
        private int LastAckTime { get; set; }
        private ConnectionStringTable StringTable { get; set; }

        #endregion Properties

        public NetConnection()
        {
            TypeFlags = new BitSet();
            ConnectionParameters = new ConnectionParameters();

            InitialSendSeq = RandomUtil.ReadI();
            RandomUtil.Read(ConnectionParameters.Nonce.Data, Nonce.NonceSize);

            SimulatedLatency = 0;
            SimulatedPacketLoss = 0.0f;

            LastPacketRecvTime = 0;
            LastUpdateTime = 0;
            RoundTripTime = 0.0f;
            SendDelayCredit = 0;
            ConnectionState = NetConnectionState.NotConnected;

            NotifyQueueHead = null;
            NotifyQueueTail = null;

            LocalRate = new NetRate
            {
                MaxRecvBandwidth = DefaultFixedBandwidth,
                MaxSendBandwidth = DefaultFixedBandwidth,
                MinPacketRecvPeriod = DefaultFixedSendPeriod,
                MinPacketSendPeriod = DefaultFixedSendPeriod
            };

            RemoteRate = LocalRate;
            ConnectLastSendTime = 0;
            LocalRateChanged = true;

// ReSharper disable DoNotCallOverridableMethodsInConstructor
            ComputeNegotiatedRate();
// ReSharper restore DoNotCallOverridableMethodsInConstructor

            PingSendCount = 0;
            LastPingSendTime = 0;

            LastSeqRecvd = 0;
            HighestAckedSeq = InitialSendSeq;
            LastSendSeq = InitialSendSeq;
            AckMask = new uint[MaxAckMaskSize];
            AckMask[0] = 0;
            LastRecvAckAck = 0;
            Cwnd = 2;
            SSThresh = 30;
            LastSeqRecvdAck = 0;

            PingTimeout = DefaultPingTimeout;
            PingRetryCount = DefaultPingRetryCount;

            //StringTable = null;
            NumNotifies = 0;

            LastSeqRecvdAtSend = new uint[MaxPacketWindowSize];
        }

        ~NetConnection()
        {
            ClearAllPacketNotifies();
        }

        private bool HasUnackedSentPackets()
        {
            return LastSendSeq != HighestAckedSeq;
        }

        public virtual void OnConnectTerminated(TerminationReason reason, string reasonString)
        {
        }

        public virtual void OnConnectionTerminated(TerminationReason reason, string reasonString)
        {
        }

        public virtual void OnConnectionEstablished()
        {
            if (IsInitiator())
                SetIsConnectionToServer();
            else
                SetIsConnectionToClient();
        }

        public virtual bool ValidateCertificate(Certificate theCertificate, bool isInitiator)
        {
            return true;
        }

        public virtual bool ValidatePublicKey(AsymmetricKey theKey, bool isInitiator)
        {
            return true;
        }

        public virtual void WriteConnectRequest(BitStream stream)
        {
            stream.Write((uint) GetNetClassGroup());
            stream.Write(NetClassRep.GetClassGroupCRC(GetNetClassGroup()));
        }

        public virtual bool ReadConnectRequest(BitStream reader, ref string errorString)
        {

            reader.Read(out uint classGroup);
            reader.Read(out uint classCRC);

            if ((NetClassGroup)classGroup == GetNetClassGroup() && classCRC == NetClassRep.GetClassGroupCRC(GetNetClassGroup()))
                return true;

            errorString = "CHR_INVALID";
            return false;
        }

        public virtual void WriteConnectAccept(BitStream stream)
        {
        }

        public virtual bool ReadConnectAccept(BitStream reader, ref string errorString)
        {
            return true;
        }

        protected virtual void ReadPacket(BitStream reader)
        {
        }

        public virtual void PrepareWritePacket()
        {
        }

        protected virtual void WritePacket(BitStream stream, PacketNotify note)
        {
        }

        protected virtual void PacketReceived(PacketNotify note)
        {
            if (StringTable != null)
                StringTable.PacketReceived(note.StringList);
        }

        protected virtual void PacketDropped(PacketNotify note)
        {
            if (StringTable != null)
                StringTable.PacketDropped(note.StringList);
        }

        protected virtual PacketNotify AllocNotify()
        {
            return new PacketNotify();
        }

        public uint GetNextSendSequence()
        {
            return LastSendSeq + 1;
        }

        public uint GetLastSendSequence()
        {
            return LastSendSeq;
        }

        public void ReadRawPacket(BitStream stream)
        {
            if (SimulatedPacketLoss > 0.0f && RandomUtil.ReadF() < SimulatedPacketLoss)
                return;

            ErrorBuffer[0] = '\0';

            if (!ReadPacketHeader(stream))
                return;

            LastPacketRecvTime = Interface.GetCurrentTime();

            ReadPacketRateInfo(stream);

            stream.SetStringTable(StringTable);

            ReadPacket(stream);

            if (!stream.IsValid() && ErrorBuffer[0] == '\0')
                SetLastError("Invalid Packet.");

            if (ErrorBuffer[0] != '\0')
            {
                var strLen = ErrorBuffer.Length;

                for (var i = 1; i < ErrorBuffer.Length; ++i)
                {
                    if (ErrorBuffer[i] != '\0')
                        continue;

                    strLen = i;
                    break;
                }

                GetInterface().HandleConnectionError(this, new string(ErrorBuffer, 0, strLen));
            }

            ErrorBuffer[0] = '\0';
        }

        protected void WriteRawPacket(BitStream stream, NetPacketType packetType)
        {
            WritePacketHeader(stream, packetType);

            if (packetType == NetPacketType.DataPacket)
            {
                var note = AllocNotify();

                ++NumNotifies;

                if (NotifyQueueHead == null)
                    NotifyQueueHead = note;
                else
                    NotifyQueueTail.NextPacket = note;

                NotifyQueueTail = note;

                note.NextPacket = null;
                note.SendTime = Interface.GetCurrentTime();

                WritePacketRateInfo(stream, note);

                //var start = stream.GetBitPosition();

                stream.SetStringTable(StringTable);

                //Console.WriteLine("NetConnection {0}: START {1}", NetAddress, GetClassName());

                WritePacket(stream, note);

                //Console.WriteLine("NetConnection {0}: END {1} - {2} bits", NetAddress, GetClassName(), stream.GetBitPosition() - start);
            }

            if (SymmetricCipher == null)
                return;

            SymmetricCipher.SetupCounter(LastSendSeq, LastSeqRecvd, (uint) packetType, 0U);

            stream.HashAndEncrypt(MessageSignatureBytes, PacketHeaderByteSize, SymmetricCipher);
        }

        protected void WritePacketHeader(BitStream stream, NetPacketType packetType)
        {
            if (WindowFull() && packetType == NetPacketType.DataPacket)
                Debugger.Break();

            var ackByteCount = ((LastSeqRecvd - LastRecvAckAck + 7) >> 3);

            if (packetType == NetPacketType.DataPacket)
                ++LastSendSeq;

            stream.WriteInt((uint) packetType, 2);
            stream.WriteInt(LastSendSeq, 5);
            stream.WriteFlag(true);
            stream.WriteInt(LastSendSeq >> 5, SequenceNumberBitSize - 5);
            stream.WriteInt(LastSeqRecvd, AckSequenceNumberBitSize);
            stream.WriteInt(0, PacketHeaderPadBits);

            stream.WriteRangedU32(ackByteCount, 0, MaxAckByteCount);

            var wordCount = (ackByteCount + 3) >> 2;

            for (var i = 0U; i < wordCount; ++i)
                stream.WriteInt(AckMask[i], (byte) (i == wordCount - 1 ? (ackByteCount - (i * 4)) * 8 : 32));

            var sendDelay = Interface.GetCurrentTime() - LastPacketRecvTime;
            if (sendDelay > 2047)
                sendDelay = 2047;

            stream.WriteInt((uint) sendDelay >> 3, 8);

            if (packetType == NetPacketType.DataPacket)
                LastSeqRecvdAtSend[LastSendSeq & PacketWindowMask] = LastSeqRecvd;
        }

        protected bool ReadPacketHeader(BitStream stream)
        {
            var packetType = (NetPacketType) stream.ReadInt(2);
            var sequenceNumber = stream.ReadInt(5);
            /*var dataPacketFlag = */stream.ReadFlag();
            sequenceNumber |= (stream.ReadInt(SequenceNumberBitSize - 5) << 5);

            var highestAck = stream.ReadInt(AckSequenceNumberBitSize);
            var padBits = stream.ReadInt(PacketHeaderPadBits);

            if (padBits != 0)
                return false;

            sequenceNumber |= (uint) (LastSeqRecvd & SequenceNumberMask);
            if (sequenceNumber < LastSeqRecvd)
                sequenceNumber += SequenceNumberWindowSize;

            if (sequenceNumber - LastSeqRecvd > (MaxPacketWindowSize - 1))
                return false;

            highestAck |= (uint) (HighestAckedSeq & AckSequenceNumberMask);
            if (highestAck < HighestAckedSeq)
                highestAck += AckSequenceNumberWindowSize;

            if (highestAck > LastSendSeq)
                return false;

            if (SymmetricCipher != null)
            {
                SymmetricCipher.SetupCounter(sequenceNumber, highestAck, (uint) packetType, 0);

                if (!stream.DecryptAndCheckHash(MessageSignatureBytes, PacketHeaderByteSize, SymmetricCipher))
                {
                    Console.WriteLine("Packet failed crypto");
                    return false;
                }
            }

            var ackByteCount = stream.ReadRangedU32(0, MaxAckByteCount);
            if (ackByteCount > MaxAckByteCount || packetType >= NetPacketType.InvalidPacketType)
                return false;

            var ackMask = new uint[MaxAckMaskSize];
            var ackWordCount = (ackByteCount + 3) >> 2;

            for (var i = 0U; i < ackWordCount; ++i)
                ackMask[i] = stream.ReadInt((byte) (i == ackWordCount - 1 ? (ackByteCount - (i * 4)) * 8 : 32));

            var sendDelay = (stream.ReadInt(8) << 3) + 4;

            var ackMaskShift = sequenceNumber - LastSeqRecvd;

            while (ackMaskShift > 32)
            {
                for (var i = MaxAckMaskSize - 1; i > 0; --i)
                    AckMask[i] = AckMask[i - 1];

                AckMask[0] = 0;
                ackMaskShift -= 32;
            }

            var upShifted = packetType == NetPacketType.DataPacket ? 1U : 0U;

            for (var i = 0U; i < MaxAckMaskSize; ++i)
            {
                var nextShift = AckMask[i] >> (int) (32 - ackMaskShift);
                AckMask[i] = (AckMask[i] << (int) ackMaskShift) | upShifted;
                upShifted = nextShift;
            }

            var notifyCount = highestAck - HighestAckedSeq;
            for (var i = 0U; i < notifyCount; ++i)
            {
                var notifyIndex = HighestAckedSeq + i + 1;

                var ackMaskBit = (highestAck - notifyIndex) & 0x1FU;
                var ackMaskWord = (highestAck - notifyIndex) >> 5;

                var packetTransmitSuccess = (ackMask[ackMaskWord] & (1U << (int) ackMaskBit)) != 0U;

                HighestAckedSendTime = 0;
                HandleNotify(notifyIndex, packetTransmitSuccess);

                if (HighestAckedSendTime > 0)
                {
                    var roundTripDelta = Interface.GetCurrentTime() - (HighestAckedSendTime + (int) sendDelay);

                    RoundTripTime = RoundTripTime * 0.9f + roundTripDelta * 0.1f;

                    if (RoundTripTime < 0.0f)
                        RoundTripTime = 0.0f;
                }

                if (packetTransmitSuccess)
                    LastRecvAckAck = LastSeqRecvdAtSend[notifyIndex & PacketWindowMask];
            }

            if (sequenceNumber - LastRecvAckAck > MaxPacketWindowSize)
                LastRecvAckAck = sequenceNumber - MaxPacketWindowSize;

            HighestAckedSeq = highestAck;
            PingSendCount = 0;
            LastPingSendTime = 0;

            KeepAlive();

            var prevLastSequence = LastSeqRecvd;
            LastSeqRecvd = sequenceNumber;

            if (packetType == NetPacketType.PingPacket) // || (sequenceNumber - LastRecvAckAck > (MaxPacketWindowSize >> 1)))
            //if (packetType == NetPacketType.PingPacket || (sequenceNumber - LastRecvAckAck > (MaxPacketWindowSize >> 1)))
                SendAckPacket();

            return prevLastSequence != sequenceNumber && packetType == NetPacketType.DataPacket;
        }

        protected void WritePacketRateInfo(BitStream stream, PacketNotify note)
        {
            note.RateChanged = LocalRateChanged;
            LocalRateChanged = false;

            if (stream.WriteFlag(note.RateChanged) && !stream.WriteFlag(TypeFlags.Test((uint) NetConnectionTypeFlags.ConnectionAdaptive)))
            {
                stream.WriteRangedU32(LocalRate.MaxRecvBandwidth, 0, MaxFixedBandwidth);
                stream.WriteRangedU32(LocalRate.MaxSendBandwidth, 0, MaxFixedBandwidth);
                stream.WriteRangedU32(LocalRate.MinPacketRecvPeriod, 1, MaxFixedSendPeriod);
                stream.WriteRangedU32(LocalRate.MinPacketSendPeriod, 1, MaxFixedSendPeriod);
            }
        }

        protected void ReadPacketRateInfo(BitStream stream)
        {
            if (stream.ReadFlag())
            {
                if (stream.ReadFlag())
                    TypeFlags.Set((uint) NetConnectionTypeFlags.ConnectionRemoteAdaptive);
                else
                {
                    RemoteRate.MaxRecvBandwidth = stream.ReadRangedU32(0, MaxFixedBandwidth);
                    RemoteRate.MaxSendBandwidth = stream.ReadRangedU32(0, MaxFixedBandwidth);
                    RemoteRate.MinPacketRecvPeriod = stream.ReadRangedU32(1, MaxFixedSendPeriod);
                    RemoteRate.MinPacketSendPeriod = stream.ReadRangedU32(1, MaxFixedSendPeriod);

                    ComputeNegotiatedRate();
                }
            }
        }

        protected void SendPingPacket()
        {
            var stream = new PacketStream();

            WriteRawPacket(stream, NetPacketType.PingPacket);

            SendPacket(stream);
        }

        protected void SendAckPacket()
        {
            var stream = new PacketStream();

            WriteRawPacket(stream, NetPacketType.AckPacket);

            SendPacket(stream);
        }

#pragma warning disable IDE0060 // Remove unused parameter
        protected void HandleNotify(uint sequence, bool recvd)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            //Console.WriteLine("NetConnection {0}: NOTIFY {1} {2}", NetAddress, sequence, recvd ? "RECVD" : "DROPPED");

            var note = NotifyQueueHead;
            NotifyQueueHead = NotifyQueueHead.NextPacket;

            if (note.RateChanged && !recvd)
                LocalRateChanged = true;

            if (recvd)
            {
                HighestAckedSendTime = note.SendTime;

                if (IsAdaptive())
                {
                    if (Cwnd < SSThresh)
                        ++Cwnd;
                    else if (Cwnd < MaxPacketWindowSize - 2.0f)
                        Cwnd += 1.0f / Cwnd;
                }

                PacketReceived(note);
                --NumNotifies;
            }
            else
            {
                if (IsAdaptive())
                {
                    SSThresh = (0.5f * SSThresh < 2.0f) ? 2.0f : (0.5f * SSThresh);
                    Cwnd -= 1.0f;

                    if (Cwnd < 2.0f)
                        Cwnd = 2.0f;
                }

                PacketDropped(note);
                --NumNotifies;
            }
        }

        protected void KeepAlive()
        {
            LastPingSendTime = 0;
            PingSendCount = 0;
        }

        protected void ClearAllPacketNotifies()
        {
            while (NotifyQueueHead != null)
                HandleNotify(0, false);
        }

        public void SetInitialRecvSequence(uint sequence)
        {
            InitialRecvSeq = LastSeqRecvd = LastRecvAckAck = sequence;
        }

        public uint GetInitialRecvSequence()
        {
            return InitialRecvSeq;
        }

        public uint GetInitialSendSequence()
        {
            return InitialSendSeq;
        }

        public void Connect(NetInterface connectionInterface, IPEndPoint address, bool requestKeyExchange = false, bool requestCertificate = false)
        {
            ConnectionParameters.RequestKeyExchange = requestKeyExchange;
            ConnectionParameters.RequestCertificate = requestCertificate;
            ConnectionParameters.IsInitiator = true;

            SetNetAddress(address);
            SetInterface(connectionInterface);

            Interface.StartConnection(this);
        }

        public bool ConnectLocal(NetInterface connectionInterface, NetInterface serverInterface)
        {
            var co = Create(GetClassName());
            if (co is not NetConnection server)
                return false;

            var client = this;
            string error = null;

            var stream = new PacketStream();

            client.SetInterface(connectionInterface);
            client.ConnectionParameters.IsInitiator = true;
            client.ConnectionParameters.IsLocal = true;
            server.ConnectionParameters.IsLocal = true;

            server.SetInterface(serverInterface);

            server.SetInitialRecvSequence(client.GetInitialSendSequence());
            client.SetInitialRecvSequence(server.GetInitialSendSequence());
            client.SetRemoteConnectionObject(server);
            server.SetRemoteConnectionObject(client);

            stream.SetBytePosition(0U);
            client.WriteConnectRequest(stream);
            stream.SetBytePosition(0U);
            if (!server.ReadConnectRequest(stream, ref error))
                return false;

            stream.SetBytePosition(0U);
            server.WriteConnectAccept(stream);
            stream.SetBytePosition(0U);

            if (!client.ReadConnectAccept(stream, ref error))
                return false;

            client.ConnectionState = NetConnectionState.Connected;
            server.ConnectionState = NetConnectionState.Connected;

            client.OnConnectionEstablished();
            server.OnConnectionEstablished();

            connectionInterface.AddConnection(client);
            serverInterface.AddConnection(server);
            return true;
        }

        public void ConnectArranged(NetInterface connectionInterface, List<IPEndPoint> possibleAddresses, Nonce myNonce, Nonce remoteNonce, ByteBuffer sharedSecret, bool isInitiator, bool requestsKeyExchange = false, bool requestsCertificate = false)
        {
            ConnectionParameters.RequestKeyExchange = requestsKeyExchange;
            ConnectionParameters.RequestCertificate = requestsCertificate;
            ConnectionParameters.PossibleAddresses = possibleAddresses;
            ConnectionParameters.IsInitiator = isInitiator;
            ConnectionParameters.IsArranged = true;
            ConnectionParameters.Nonce = myNonce;
            ConnectionParameters.ServerNonce = remoteNonce;
            ConnectionParameters.ArrangedSecret = sharedSecret;

            SetInterface(connectionInterface);

            Interface.StartArrangedConnection(this);
        }

        public void Disconnect(string reason)
        {
            if(Interface != null)
                Interface.Disconnect(this, TerminationReason.ReasonSelfDisconnect, reason);
        }

        public bool WindowFull()
        {
            if (LastSendSeq - HighestAckedSeq >= (MaxPacketWindowSize - 2))
                return true;

            if (IsAdaptive())
                return LastSendSeq - HighestAckedSeq >= Cwnd;

            return false;
        }

        protected virtual void ComputeNegotiatedRate()
        {
            CurrentPacketSendPeriod = Math.Max(LocalRate.MinPacketSendPeriod, RemoteRate.MinPacketRecvPeriod);

            var maxBandwith = Math.Min(LocalRate.MaxSendBandwidth, RemoteRate.MaxRecvBandwidth);
            CurrentPacketSendSize = (uint) (maxBandwith * CurrentPacketSendPeriod * 0.001f);

            if (CurrentPacketSendSize > MaxPacketDataSize)
                CurrentPacketSendSize = MaxPacketDataSize;
        }

        public void SetConnectionParameters(ConnectionParameters parameters)
        {
            ConnectionParameters = parameters;
        }

        public ConnectionParameters GetConnectionParameters()
        {
            return ConnectionParameters;
        }

        public bool IsInitiator()
        {
            return ConnectionParameters.IsInitiator;
        }

        public void SetRemoteConnectionObject(NetConnection connection)
        {
            RemoteConnection = connection;
        }

        public NetConnection GetRemoteConnectionObject()
        {
            return RemoteConnection;
        }

        public static char[] GetErrorBuffer()
        {
            return ErrorBuffer;
        }

        public static void SetLastError(string fmt, params object[] args)
        {
            var str = string.Format(fmt, args);

            Array.Copy(str.ToCharArray(), ErrorBuffer, str.Length > ErrorBuffer.Length ? ErrorBuffer.Length : str.Length);
        }

        public void SetInterface(NetInterface myInterface)
        {
            Interface = myInterface;
        }

        public NetInterface GetInterface()
        {
            return Interface;
        }

        public void SetSymmetricCipher(SymmetricCipher theCipher)
        {
            SymmetricCipher = theCipher;
        }

        public virtual NetClassGroup GetNetClassGroup()
        {
            return NetClassGroup.NetClassGroupInvalid;
        }

        public void SetPingTimeouts(int msPerPing, int pingRetryCount)
        {
            PingRetryCount = pingRetryCount;
            PingTimeout = msPerPing;
        }

        public void SetSimulatedNetParams(int packetLoss, int latency)
        {
            SimulatedPacketLoss = packetLoss;
            SimulatedLatency = latency;
        }

        public void SetIsConnectionToServer()
        {
            TypeFlags.Set((uint) NetConnectionTypeFlags.ConnectionToServer);
        }

        public bool IsConnectionToServer()
        {
            return TypeFlags.Test((uint) NetConnectionTypeFlags.ConnectionToServer);
        }

        public void SetIsConnectionToClient()
        {
            TypeFlags.Set((uint) NetConnectionTypeFlags.ConnectionToClient);
        }

        public bool IsConnectionToClient()
        {
            return TypeFlags.Test((uint) NetConnectionTypeFlags.ConnectionToClient);
        }

        public bool IsLocalConnection()
        {
            return RemoteConnection != null;
        }

        public bool IsNetworkConnection()
        {
            return RemoteConnection == null;
        }

        public float GetRoundTripTime()
        {
            return RoundTripTime;
        }

        public float GetOneWayTime()
        {
            return RoundTripTime * 0.5F;
        }

        public IPEndPoint GetNetAddress()
        {
            return NetAddress;
        }

        public string GetNetAddressString()
        {
            return NetAddress.ToString();
        }

        public void SetNetAddress(IPEndPoint address)
        {
            NetAddress = address;
        }

        public NetError SendPacket(BitStream stream)
        {
            if (SimulatedPacketLoss > 0.0f && RandomUtil.ReadF() < SimulatedPacketLoss)
                return NetError.NoError;

            if (IsLocalConnection())
            {
                var size = stream.GetBytePosition();

                stream.Reset();
                stream.SetMaxSizes(size, 0);

                RemoteConnection.ReadRawPacket(stream);
                return NetError.NoError;
            }

            if (SimulatedLatency > 0.0f)
            {
                Interface.SendToDelayed(GetNetAddress(), stream, SimulatedLatency);
                return NetError.NoError;
            }

            return Interface.SendTo(GetNetAddress(), stream);
        }

        public bool CheckTimeout(int time)
        {
            if (!IsNetworkConnection())
                return false;

            if (LastPingSendTime == 0)
                LastPingSendTime = time;

            var timeout = PingTimeout;
            var timeoutCount = PingRetryCount;

            if (IsAdaptive())
            {
                if (HasUnackedSentPackets())
                    timeout = AdaptiveUnackedSentPingTimeout;
                else
                {
                    timeoutCount = AdaptivePingRetryCount;

                    if (PingSendCount == 0)
                        timeout = AdaptiveInitialPingTimeout;
                }
            }

            if ((time - LastPingSendTime) > timeout)
            {
                if (PingSendCount >= timeoutCount)
                    return true;

                LastPingSendTime = time;
                ++PingSendCount;

                SendPingPacket();
            }

            return false;
        }

        public void CheckPacketSend(bool force, int curTime)
        {
            var delay = CurrentPacketSendPeriod;

            if (!force)
            {
                if (!IsAdaptive())
                {
                    if (curTime - LastUpdateTime + SendDelayCredit < delay)
                        return;

                    SendDelayCredit = (uint) (curTime - (LastUpdateTime + delay - SendDelayCredit));
                    if (SendDelayCredit > 1000U)
                        SendDelayCredit = 1000U;
                }
            }

            PrepareWritePacket();

            if (WindowFull() || !IsDataToTransmit())
            {
                if (IsAdaptive())
                {
                    var ackDelta = (LastSeqRecvd - LastSeqRecvdAck);
                    var ack = ackDelta / 4.0f;

                    var deltaT = (curTime - LastAckTime);
                    ack = ack * deltaT / 200.0f;

                    if ((ack > 1.0f || (ackDelta > (0.75f * MaxPacketWindowSize))) && LastSeqRecvdAck != LastSeqRecvd)
                    {
                        LastSeqRecvdAck = LastSeqRecvd;
                        LastAckTime = curTime;

                        SendAckPacket();
                    }
                }

                return;
            }

            var stream = new PacketStream(CurrentPacketSendSize);

            LastUpdateTime = curTime;

            WriteRawPacket(stream, NetPacketType.DataPacket);

            SendPacket(stream);
        }

        public void SetConnectionState(NetConnectionState state)
        {
            ConnectionState = state;
        }

        public NetConnectionState GetConnectionState()
        {
            return ConnectionState;
        }

        public bool IsEstablished()
        {
            return ConnectionState == NetConnectionState.Connected;
        }

        public void SetIsAdaptive()
        {
            TypeFlags.Set((uint) NetConnectionTypeFlags.ConnectionAdaptive);
            LocalRateChanged = true;
        }

        public void SetFixedRateParameters(uint minPacketSendPeriod, uint minPacketRecvPeriod, uint maxSendBandwidth, uint maxRecvBandwidth)
        {
            TypeFlags.Clear((uint) NetConnectionTypeFlags.ConnectionAdaptive);

            LocalRate.MaxRecvBandwidth = maxRecvBandwidth;
            LocalRate.MaxSendBandwidth = maxSendBandwidth;
            LocalRate.MinPacketRecvPeriod = minPacketRecvPeriod;
            LocalRate.MinPacketSendPeriod = minPacketSendPeriod;
            LocalRateChanged = true;

            ComputeNegotiatedRate();
        }

        public bool IsAdaptive()
        {
            return TypeFlags.Test((uint) (NetConnectionTypeFlags.ConnectionAdaptive | NetConnectionTypeFlags.ConnectionRemoteAdaptive));
        }

        public virtual bool IsDataToTransmit()
        {
            return false;
        }

        public void SetTranslatesStrings()
        {
            if (StringTable == null)
                StringTable = new ConnectionStringTable(this);
        }

        public static void ImplementClass<T>(out NetClassRepInstance<T> rep) where T : BaseObject, new()
        {
            rep = new NetClassRepInstance<T>(typeof(T).Name, 0, NetClassType.NetClassTypeNone, 0);
        }

        public static void ImplementNetConnection<T>(out NetClassRepInstance<T> rep, out NetConnectionRep connRep, bool canRemoteCreate) where T : BaseObject, new()
        {
            ImplementClass(out rep);

            connRep = new NetConnectionRep(rep, canRemoteCreate);
        }
    }
}
