using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public struct Effect
    {
        public uint EntityId;
        public Weapon Weapon;
        public RoomSessionPlayer Target;
    }
}
