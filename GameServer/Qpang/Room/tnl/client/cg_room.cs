using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNL.Entities;
using TNL.Utils;

namespace Qserver.GameServer.Qpang
{
    public class CGRoom : GameNetEvent
    {
        public CGRoom() : base(GameNetId.CG_ROOM, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer)
        {

        }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps)
        {
            Post<CGRoom>(ps); // TODO: What the actual fuck?
        }
        public void Handle(EventConnection ps, Player player)
        {

        }

    }
    
}
