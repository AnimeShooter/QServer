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
        public ushort[] UnkA;
        public ushort[] UnkB;
        //public uint Unk3; // 96 ??
        //public uint Unk4; // 100
        //public uint Unk5; // 104
        //public uint Unk6; // 108

        public GCPvEMoveNodesNpc(uint unk1, ushort[] unka, ushort[] unkb) : base(GameNetId.GC_PVE_MOVE_NODES_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            Unk1 = unk1;
            UnkA = unka;
            UnkB = unkb;
        }
        public GCPvEMoveNodesNpc() : base(GameNetId.GC_PVE_MOVE_NODES_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(Unk1);
            bitStream.Write(UnkA.Length);
            for(int i = 0; i < UnkCount; i++)
            {
                bitStream.Write((ushort)UnkA[i]);
                bitStream.Write((ushort)UnkB[i]);
            }
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Unk1);
            bitStream.Read(out UnkCount);
            UnkA = new ushort[UnkCount];
            UnkB = new ushort[UnkCount];
            for(int i = 0; i < UnkCount; i++)
            {
                ushort unka;
                ushort unkb;
                bitStream.Read(out unka);
                bitStream.Read(out unkb);
                UnkA[i] = unka;
                UnkB[i] = unkb;
            }
        }
        public override void Process(EventConnection ps) { }
    }
}
