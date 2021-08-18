using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class GreenMedKit : GameItem
    {
        public override uint OnPickUp(RoomSessionPlayer session)
        {
            session.HealTeam(50);
            return 0;
        }
    }
}
