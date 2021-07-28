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
    public class CGCharm : GameNetEvent
    {
        private static NetClassRepInstance<CGCharm> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGCharm", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint unk01;
        public uint unk02;

        public CGCharm() : base(GameNetId.CG_CHARM, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out unk01);
            bitStream.Read(out unk02);
        }
        public override void Process(EventConnection ps) { }
    }
}
