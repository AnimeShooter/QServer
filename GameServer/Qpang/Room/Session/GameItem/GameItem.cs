using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class GameItem
    {
        public virtual uint OnPickUp(RoomSessionPlayer player)
        {
            return 0;
        }
    }
}
