using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Room.Session.Player.Entity
{
    public class PlayerBaseEntity
    {
        private uint _id;
        private uint _killCount;

        public uint Id
        {
            get { return _id; }
        }

        public PlayerBaseEntity(uint id)
        {
            this._id = id;
            this._killCount = 0;
        }

    }
}
