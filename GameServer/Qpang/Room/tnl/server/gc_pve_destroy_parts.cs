using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNL.Entities;
using TNL.Utils;
using TNL.Data;
using TNL.Types;

namespace Qserver.GameServer.Qpang
{
    public class GCPvEDestroyParts : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEDestroyParts> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEDestroyParts", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint NpcUid; // 88 Uid?
        public uint BodyPartUid; // 92 Part?

        public GCPvEDestroyParts() : base(GameNetId.GC_PVE_DESTROY_PARTS, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(NpcUid);
            bitStream.Write(BodyPartUid);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out NpcUid);
            bitStream.Read(out BodyPartUid);
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }
    }
}
