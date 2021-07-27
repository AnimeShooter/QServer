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
    public class GCEssence : GameNetEvent
    {
        private static NetClassRepInstance<GCEssence> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCEssence", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint unk01;
        public uint unk02;
        public uint unk03;
        public uint unk04;
        public uint unk05;
        public uint unk06;
        public uint unk07;
        public uint unk08;
        public uint unk09;
        public uint IsP2P;

        public GCEssence() : base(GameNetId.CG_ESSENCE, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }
        public GCEssence(uint unk01, uint unk02, uint unk03, uint unk04, uint unk05, uint unk06, uint unk07, uint unk08, uint unk09) : base(GameNetId.CG_ESSENCE, GuaranteeType.GuaranteedOrdered, EventDirection.DirServerToClient)
        {
            this.unk01 = unk01;
            this.unk02 = unk02;
            this.unk03 = unk03;
            this.unk04 = unk04;
            this.unk05 = unk05;
            this.unk06 = unk06;
            this.unk07 = unk07;
            this.unk08 = unk08;
            this.unk09 = unk09;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(unk01);
            bitStream.Write(unk02);
            bitStream.Write(unk03);
            bitStream.Write(unk04);
            bitStream.Write(unk05);
            bitStream.Write(unk06);
            bitStream.Write(unk07);
            bitStream.Write(unk08);
            bitStream.Write(unk09);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
