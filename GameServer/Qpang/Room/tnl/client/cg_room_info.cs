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
    public class CGRoomInfo : GameNetEvent
    {
        private static NetClassRepInstance<CGRoomInfo> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGRoomInfo", NetClassMask.NetClassGroupGameMask, 0);
        }

        public byte MasterUid; // 88

        public CGRoomInfo() : base(GameNetId.CG_ROOM_INFO, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }



        public override void Pack(EventConnection ps, BitStream bitStream)
        {
        }

        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out MasterUid);
        }
        public override void Process(EventConnection ps) { }
    }
}
