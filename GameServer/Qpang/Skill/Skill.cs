using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class Skill
    {
        private RoomSessionPlayer _player;
        private RoomSessionPlayer _target;
        private uint _startTime;

        public RoomSessionPlayer Player
        {
            get { return this._player; }
        }

        public RoomSessionPlayer Target
        {
            get { return this._target; }
            set { this._target = value; }
        }

        public uint StartTime
        {
            get { return this._startTime; }
        }

        public uint Duration
        {
            get { return GetDuration(); }
        }

        public uint Id
        {
            get { return GetId(); }
        }

        public virtual uint GetId() 
        {
            return 0;
        }

        public virtual uint GetDuration()
        {
            return 15; // default?
        }

        public void Bind(RoomSessionPlayer player)
        {
            this._player = player;
        }

        public virtual void OnUse(RoomSessionPlayer target)
        {
            this._target = target;
            this._startTime = Util.Util.Timestamp();
        }
    }
}
