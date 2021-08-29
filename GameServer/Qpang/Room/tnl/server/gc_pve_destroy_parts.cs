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

        public uint Unk1; // 88 Uid?
        public uint Unk2; // 92 Part?

        public GCPvEDestroyParts() : base(GameNetId.GC_PVE_DESTROY_PARTS, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(Unk1);
            bitStream.Write(Unk2);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Unk1);
            bitStream.Read(out Unk2);
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }
    }
}
