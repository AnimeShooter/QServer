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
    public class CGPvEFloor : GameNetEvent
    {
        private static NetClassRepInstance<CGPvEFloor> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGPvEFloor", NetClassMask.NetClassGroupGameMask, 0);
        }

        public byte Flag;

        public CGPvEFloor() : base(GameNetId.CG_PVE_FLOOR, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Flag);
        }
        public override void Process(EventConnection ps) { }
    }
}
