namespace TNL.Entities
{
    using Data;
    using Types;
    using Utils;

    public enum EventDirection
    {
        DirUnset,
        DirAny,
        DirServerToClient,
        DirClientToServer
    }

    public enum GuaranteeType
    {
        GuaranteedOrdered = 0,
        Guaranteed = 1,
        Unguaranteed = 2
    }

    public abstract class NetEvent : BaseObject
    {
        public EventDirection EventDirection { get; private set; }
        public GuaranteeType GuaranteeType { get; private set; }

        protected NetEvent(GuaranteeType gType = GuaranteeType.GuaranteedOrdered, EventDirection evDir = EventDirection.DirUnset)
        {
            GuaranteeType = gType;
            EventDirection = evDir;
        }

        public abstract void Pack(EventConnection ps, BitStream stream);
        public abstract void Unpack(EventConnection ps, BitStream steam);
        public abstract void Process(EventConnection ps);

        public virtual void NotifyPosted(EventConnection ps) { }
        public virtual void NotifySent(EventConnection ps) { }
        public virtual void NotifyDelivered(EventConnection ps, bool madeId) { }

        public EventDirection GetEventDirection()
        {
            return EventDirection;
        }

        public virtual string GetDebugName()
        {
            return GetClassName();
        }

        public static void ImplementNetEvent<T>(out NetClassRepInstance<T> rep, string name, NetClassMask groupMask, int classVersion) where T : NetEvent, new()
        {
            rep = new NetClassRepInstance<T>(name, (uint) groupMask, NetClassType.NetClassTypeEvent, classVersion);
        }
    }
}
