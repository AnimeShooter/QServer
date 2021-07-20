namespace TNL.Notify
{
    using Structures;

    public class PacketNotify
    {
        public bool RateChanged { get; set; }
        public int SendTime { get; set; }
        public ConnectionStringTable.PacketList StringList { get; set; }
        public PacketNotify NextPacket { get; set; }

        public PacketNotify()
        {
            RateChanged = false;
            SendTime = 0;
        }
    }
}
