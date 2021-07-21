using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNL.Entities;
using TNL.Utils;

namespace Qserver.GameServer.Qpang
{
    public class GCArrangedConn : GameNetEvent
    {
        public GCArrangedConn() : base(GameNetId.GC_ARRANGED_CONN, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
