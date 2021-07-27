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
    public class GCDisconnect : GameNetEvent
    {
        private static NetClassRepInstance<GCDisconnect> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCDisconnect", NetClassMask.NetClassGroupGameMask, 0);
        }
        public GCDisconnect() : base(GameNetId.CG_DISCONNECT, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { } // cg, gc whatever?

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
