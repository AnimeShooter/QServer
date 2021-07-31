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
    public class CGPvEHitN2P : GameNetEvent
    {
        private static NetClassRepInstance<CGPvEHitN2P> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGPvEHitN2P", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk01;
        public uint Unk02;
        public uint Unk03;
        public uint Unk04;
        public uint Unk05;
        public uint Unk06;
        public uint Unk07;
        public uint Unk08;
        public byte Unk09;
        public byte Unk10;
        public ulong Unk11;

        public CGPvEHitN2P() : base(GameNetId.CG_PVE_HIT_N2P, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

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
        }
        public override void Process(EventConnection ps) { }
    }
}
