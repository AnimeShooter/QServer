using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNL.Entities;
using TNL.Utils;

namespace Qserver.GameServer.Qpang
{
    public class CGRoomInfo : GameNetEvent
    {
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
