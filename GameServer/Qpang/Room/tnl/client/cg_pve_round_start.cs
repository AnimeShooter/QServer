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
    public class CGPvERoundStart : GameNetEvent
    {
        private static NetClassRepInstance<CGPvERoundStart> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGPvERoundStart", NetClassMask.NetClassGroupGameMask, 0);
        }
        public CGPvERoundStart() : base(GameNetId.CG_PVE_ROUND_START, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) 
        {
            Post(ps);
        }

        public override void Handle(GameConnection conn, Player player)
        {
            base.Handle(conn, player);
        }
    }
}
