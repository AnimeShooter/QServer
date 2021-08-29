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
    public class GCPvEDieNpc : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEDieNpc> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEDieNpc", NetClassMask.NetClassGroupGameMask, 0);
        }

        // NOTE: this is final boss kill?
        public uint Unk1; // 88

        public GCPvEDieNpc() : base(GameNetId.GC_PVE_DIE_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCPvEDieNpc(uint uid) : base(GameNetId.GC_PVE_DIE_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            Unk1 = uid;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Unk1);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out Unk1);
        }
        public override void Process(EventConnection ps) { }
    }
}
