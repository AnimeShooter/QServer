using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class Skill
    {
        private RoomSessionPlayer _player;

        public uint Id
        {
            get { return GetId(); }
        }

        public virtual uint GetId() 
        {
            return 0;
        }

        public void Bind(RoomSessionPlayer player)
        {
            this._player = player;
        }
    }
}
