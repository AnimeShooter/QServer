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

        public uint NpcUid; // 88 Uid?
        public ushort X; // 92 Node/State?
        public ushort Y; // 94

        public GCPvEMoveNpc(uint unk1, ushort unk2, ushort unk3) : base(GameNetId.GC_PVE_MOVE_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny) 
        {
            NpcUid = unk1;
            X = unk2;
            Y = unk3;
        }

        public GCPvEMoveNpc() : base(GameNetId.GC_PVE_MOVE_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(NpcUid);
            bitStream.Write(X);
            bitStream.Write(Y);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out NpcUid);
            bitStream.Read(out X);
            bitStream.Read(out Y);
        }
        public override void Process(EventConnection ps) { }
    }
}
