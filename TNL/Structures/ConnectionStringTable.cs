using System;

namespace TNL.Structures
{
    using Entities;
    using Utils;

    public class ConnectionStringTable
    {
        public const uint EntryCount = 1024U;
        public const uint EntryBitSize = 10U;

        public ConnectionStringTable(NetConnection parent)
        {
            throw new NotImplementedException();
        }

        public void PacketReceived(PacketList packetList)
        {
            throw new NotImplementedException();
        }

        public void PacketDropped(PacketList packetList)
        {
            throw new NotImplementedException();
        }

        public void WriteStringTableEntry(BitStream bitStream, StringTableEntry ste)
        {
            throw new NotImplementedException();
        }

        public StringTableEntry ReadStringTableEntry(BitStream bitStream)
        {
            throw new NotImplementedException();
        }

        public class PacketEntry
        {
            public PacketEntry NextInPacket { get; set; }
            public Entry StringTableEntry { get; set; }
            public StringTableEntry String { get; set; }
        }

        public class PacketList
        {
            public PacketEntry StringHead { get; set; }
            public PacketEntry StringTail { get; set; }

            public PacketList()
            {
                StringHead = StringTail = null;
            }
        }

        public class Entry
        {
            public StringTableEntry String { get; set; }

            public uint Index { get; set; }
            public Entry NextHash { get; set; }
            public Entry NextLink { get; set; }
            public Entry PrevLink { get; set; }

            public bool ReceiveConfirmed { get; set; }
        }
    }
}
