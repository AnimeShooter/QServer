using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNL.Entities;
using TNL.Types;
using TNL.Utils;
using TNL.Data;
using TNL.Entities;
using TNL.Types;

namespace Qserver.GameServer.Qpang
{
    public class CGArrangedComplete : GameNetEvent
    {
        private static NetClassRepInstance<CGArrangedComplete> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGArrangedComplete", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerUid; // 88
        public uint TargetUid; // 92

        public CGArrangedComplete() : base(GameNetId.CG_ARRANGED_COMPLETE, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }
        public CGArrangedComplete(uint playerUid, uint targetUid) : base(GameNetId.CG_ARRANGED_COMPLETE, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) 
        {
            PlayerUid = playerUid;
            TargetUid = targetUid;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(PlayerUid);
            bitStream.Write(TargetUid);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out PlayerUid);
            bitStream.Read(out TargetUid);
        }
        public override void Process(EventConnection ps) { }

    }
}
