namespace TNL.Structures
{
    public class NetRate
    {
        public uint MinPacketSendPeriod { get; set; }
        public uint MinPacketRecvPeriod { get; set; }
        public uint MaxSendBandwidth { get; set; }
        public uint MaxRecvBandwidth { get; set; }
    }
}
