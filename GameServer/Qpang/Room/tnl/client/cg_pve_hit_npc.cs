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
    public class CGPvEHitNpc : GameNetEvent
    {
        private static NetClassRepInstance<CGPvEHitNpc> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGPvEHitNpc", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk01;
        public uint Unk02;
        public uint Unk03;
        public uint Unk04;
        public uint Unk05;
        public uint Unk06;
        public uint Unk07;
        public uint Unk08;
        public uint Unk09;
        public uint Unk10;
        public byte Unk11;
        public byte Unk12;
        public uint Unk13;
        public ulong Unk14;
        public byte Unk15;
        public uint Unk16;
        public byte Unk17;
        public byte Unk18;
        public ulong Unk19;

        public CGPvEHitNpc() : base(GameNetId.CG_PVE_HIT_NPC, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Unk01);
            bitStream.Read(out Unk02);
            bitStream.Read(out Unk03);
            bitStream.Read(out Unk04);
            bitStream.Read(out Unk05);
            bitStream.Read(out Unk06);
            bitStream.Read(out Unk07);
            bitStream.Read(out Unk08);
            bitStream.Read(out Unk09);
            bitStream.Read(out Unk10);
            bitStream.Read(out Unk11);
            bitStream.Read(out Unk12);
            bitStream.Read(out Unk13);
            bitStream.Read(out Unk14);
            bitStream.Read(out Unk15);
            bitStream.Read(out Unk16);
            bitStream.Read(out Unk17);
            bitStream.Read(out Unk18);
            bitStream.Read(out Unk19);
        }
        public override void Process(EventConnection ps) { }
    }
}
