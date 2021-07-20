namespace TNL.Entities
{
    using Data;
    using Structures;
    using Types;
    using Utils;

    public enum NetFlag
    {
        IsGhost       = 2,
        ScopeLocal    = 4,
        Ghostable     = 8,
        MaxNetFlagBit = 15
    }

    public enum NetObjectRPCDirection
    {
        RPCToGhost       = 1,
        RPCToGhostParent = 2
    }

    public class NetObject : BaseObject
    {
        private static NetObject _dirtyList;

#pragma warning disable CA2211 // Non-constant fields should not be visible
        protected static GhostConnection RPCSourceConnection;
        protected static GhostConnection RPCDestConnection;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        private NetObject _prevDirtyList;
        private NetObject _nextDirtyList;
        private ulong _dirtyMaskBits;
        private uint _netIndex;
        private GhostInfo _firstObjectRef;
        private NetObject _serverObject;

        protected GhostConnection OwningConnection;
        protected BitSet NetFlags;

        public static bool PIsInitialUpdate;

        static NetObject()
        {
            RPCSourceConnection = null;
            RPCDestConnection = null;
            PIsInitialUpdate = false;
            _dirtyList = null;
        }

        public NetObject()
        {
            _netIndex = 0xFFFFFFFFU;
            _firstObjectRef = null;
            _prevDirtyList = null;
            _nextDirtyList = null;
            _dirtyMaskBits = 0UL;
        }

        ~NetObject()
        {
            while (_firstObjectRef != null)
                _firstObjectRef.Connection.DetachObject(_firstObjectRef);

            if (_dirtyMaskBits > 0)
            {
                if (_prevDirtyList != null)
                    _prevDirtyList._nextDirtyList = _nextDirtyList;
                else
                    _dirtyList = _nextDirtyList;

                if (_nextDirtyList != null)
                    _nextDirtyList._prevDirtyList = _prevDirtyList;
            }
        }

        public bool IsInitialUpdate()
        {
            return PIsInitialUpdate;
        }

        public static void CollapseDirtyList()
        {
            for (var obj = _dirtyList; obj != null;)
            {
                var next = obj._nextDirtyList;
                var orMask = obj._dirtyMaskBits;

                obj._nextDirtyList = null;
                obj._prevDirtyList = null;
                obj._dirtyMaskBits = 0UL;

                if (orMask > 0)
                {
                    for (var walk = obj._firstObjectRef; walk != null; walk = walk.NextObjectRef)
                    {
                        if (walk.UpdateMask == 0)
                        {
                            walk.UpdateMask = orMask;
                            walk.Connection.GhostPushNonZero(walk);
                        }
                        else
                            walk.UpdateMask |= orMask;
                    }
                }

                obj = next;
            }

            _dirtyList = null;
        }

        public static void SetRPCSourceConnection(GhostConnection connection)
        {
            RPCSourceConnection = connection;
        }

        public static GhostConnection GetRPCSourceConnection()
        {
            return RPCSourceConnection;
        }

        public static void SetRPCDestConnection(GhostConnection destConnection)
        {
            RPCDestConnection = destConnection;
        }

        public static GhostConnection GetRPCDestConnection()
        {
            return RPCDestConnection;
        }

        public virtual bool OnGhostAdd(GhostConnection theConnection)
        {
            return true;
        }

        public virtual void OnGhostRemove()
        {
        }

        public virtual void OnGhostAvailable(GhostConnection theConnection)
        {
        }

        public void SetMaskBits(ulong orMask)
        {
            if (_dirtyMaskBits == 0UL)
            {
                if (_dirtyList != null)
                {
                    _nextDirtyList = _dirtyList;
                    _dirtyList._prevDirtyList = this;
                }

                _dirtyList = this;
            }

            _dirtyMaskBits |= orMask;
        }

        public void ClearMaskBits(ulong orMask)
        {
            if (_dirtyMaskBits > 0)
            {
                _dirtyMaskBits &= ~orMask;

                if (_dirtyMaskBits == 0)
                {
                    if (_prevDirtyList != null)
                        _prevDirtyList._nextDirtyList = _nextDirtyList;
                    else
                        _prevDirtyList = _nextDirtyList;

                    if (_nextDirtyList != null)
                        _nextDirtyList._prevDirtyList = _prevDirtyList;

                    _nextDirtyList = _prevDirtyList = null;
                }
            }

            for (var walk = _firstObjectRef; walk != null; walk = walk.NextObjectRef)
            {
                if (walk.UpdateMask > 0 && walk.UpdateMask == orMask)
                {
                    walk.UpdateMask = 0;
                    walk.Connection.GhostPushToZero(walk);
                }
                else
                    walk.UpdateMask &= ~orMask;
            }
        }

        public virtual float GetUpdatePriority(NetObject scopeObject, ulong updateMask, int updateSkips)
        {
            return updateSkips * 0.1f;
        }

        public virtual ulong PackUpdate(GhostConnection connection, ulong updateMask, BitStream stream)
        {
            return 0UL;
        }

        public virtual void UnpackUpdate(GhostConnection connection, BitStream stream)
        {
        }

        public virtual void PerformScopeQuery(GhostConnection connection)
        {
            connection.ObjectInScope(this);
        }

        public uint GetNetIndex()
        {
            return _netIndex;
        }

        public bool IsGhost()
        {
            return NetFlags.Test((uint) NetFlag.IsGhost);
        }

        public bool IsScopeLocal()
        {
            return NetFlags.Test((uint) NetFlag.ScopeLocal);
        }

        public bool IsGhostable()
        {
            return NetFlags.Test((uint) NetFlag.Ghostable);
        }

        public void PostRPCEvent(NetObjectRPCEvent theEvent)
        {
            if (IsGhost())
                OwningConnection.PostNetEvent(theEvent);
            else if (GetRPCDestConnection() != null)
                GetRPCDestConnection().PostNetEvent(theEvent);
            else
            {
                for (var walk = _firstObjectRef; walk != null; walk = walk.NextObjectRef)
                    if ((walk.Flags & (uint) GhostInfoFlags.NotAvailable) == 0)
                        walk.Connection.PostNetEvent(theEvent);
            }
        }

        public static void ImplementNetObject<T>(out NetClassRepInstance<T> rep) where T : NetObject, new()
        {
            rep = new NetClassRepInstance<T>(typeof(T).Name, (uint) NetClassMask.NetClassGroupGameMask, NetClassType.NetClassTypeObject, 0);
        }

        public void SetOwningConnection(GhostConnection ev)
        {
            OwningConnection = ev;
        }

        public void SetNetFlags(NetFlag flag)
        {
            NetFlags.Clear();
            NetFlags.Set((uint) flag);
        }

        public void SetNetIndex(uint index)
        {
            _netIndex = index;
        }

        public void SetServerObject(NetObject o)
        {
            _serverObject = o;
        }

        public void SetFirstObjectRef(GhostInfo info)
        {
            _firstObjectRef = info;
        }

        public GhostInfo GetFirstObjectRef()
        {
            return _firstObjectRef;
        }
    }
}
