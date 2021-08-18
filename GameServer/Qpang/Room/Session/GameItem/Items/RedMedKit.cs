using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class RedMedKit : GameItem
    {
        public override uint OnPickUp(RoomSessionPlayer session)
        {
            session.AddHealth(50, true);
            return 0;
        }
    }
}
