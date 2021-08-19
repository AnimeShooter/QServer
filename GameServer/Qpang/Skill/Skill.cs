using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class Skill
    {
        private RoomSessionPlayer _player;
        private RoomSessionPlayer _target;

        public RoomSessionPlayer Player
        {
            get { return this._player; }
        }

        public RoomSessionPlayer Target
        {
            get { return this._target; }
            set { this._target = value; }
        }

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
