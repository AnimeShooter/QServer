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
    public class CGLog : GameNetEvent
    {
        private static NetClassRepInstance<CGLog> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGLog", NetClassMask.NetClassGroupGameMask, 0);
        }

        public string Logbuffer;

        public CGLog() : base(GameNetId.CG_LOG, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.ReadString(out Logbuffer);
        }
        public override void Process(EventConnection ps) { }
    }
}
