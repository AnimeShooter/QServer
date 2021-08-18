using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class EventItem : GameItem
    {
        public override uint OnPickUp(RoomSessionPlayer session)
        {
            session.AddEventItemPickup();
            return 0;
        }
    }
}
