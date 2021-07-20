using System;

namespace TNL.Structures
{
    using Entities;

    [Flags]
    public enum GhostInfoFlags
    {
        InScope = 1,
        ScopeLocalAlways = 2,
        NotYetGhosted = 4,
        Ghosting = 8,
        KillGhost = 16,
        KillingGhost = 32,

        NotAvailable = (NotYetGhosted | Ghosting | KillGhost | KillingGhost)
    }

    public class GhostInfo
    {
        public NetObject Obj { get; set; }
        public ulong UpdateMask { get; set; }
        public GhostRef LastUpdateChain { get; set; }
        public GhostInfo NextObjectRef { get; set; }
        public GhostInfo PrevObjectRef { get; set; }
        public GhostConnection Connection { get; set; }
        public uint UpdateSkipCount { get; set; }
        public uint Flags { get; set; }
        public float Priority { get; set; }
        public uint Index { get; set; }
        public int ArrayIndex { get; set; }
    }
}
