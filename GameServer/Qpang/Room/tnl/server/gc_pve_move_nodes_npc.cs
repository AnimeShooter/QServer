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
    public class GCPvEMoveNodesNpc : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEMoveNodesNpc> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEMoveNodesNpc", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk1; // 88
        public uint UnkCount; // 92
        public uint Unk3; // 96 ??
        public uint Unk4; // 100
        public uint Unk5; // 104
        public uint Unk6; // 108

        public GCPvEMoveNodesNpc() : base(GameNetId.GC_PVE_MOVE_NODES_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(Unk1);
            bitStream.Write(UnkCount);
            for(int i = 0; i < UnkCount; i++)
            {
                // TODO
                bitStream.Write((uint)0);
                bitStream.Write((ushort)0);
            }
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Unk1);
            bitStream.Read(out UnkCount);
            for(int i = 0; i < UnkCount; i++)
            {
                // TODO
                uint unk1;
                ushort unk2;
                bitStream.Read(out unk1);
                bitStream.Read(out unk2);
            }
        }
        public override void Process(EventConnection ps) { }
    }
}
