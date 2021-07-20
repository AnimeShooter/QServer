namespace TNL.Notify
{
    using Entities;

    public class EventPacketNotify : PacketNotify
    {
        public EventNote EventList { get; set; }

        public EventPacketNotify()
        {
            EventList = null;
        }
    }
}
