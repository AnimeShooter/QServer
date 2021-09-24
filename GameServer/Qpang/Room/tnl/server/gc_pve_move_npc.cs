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
    public class GCPvEMoveNpc : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEMoveNpc> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEMoveNpc", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk1; // 88 Uid?
        public ushort Unk2; // 92 Node/State?
        public ushort Unk3; // 94

        public GCPvEMoveNpc(uint unk1, ushort unk2, ushort unk3) : base(GameNetId.GC_PVE_MOVE_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny) 
        {
            Unk1 = unk1;
            Unk2 = unk2;
            Unk3 = unk3;
        }

        public GCPvEMoveNpc() : base(GameNetId.GC_PVE_MOVE_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Unk1);
            bitStream.Write(Unk2);
            bitStream.Write(Unk3);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out Unk1);
            bitStream.Read(out Unk2);
            bitStream.Read(out Unk3);
        }
        public override void Process(EventConnection ps) { }
    }
}
