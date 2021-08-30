using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class PvEEntity
    {
        private uint _uid;
        private Position _position;
        
        public uint Uid
        {
            get { return this._uid; }
            // set { this._uid = value; }
        }
        public Position Position
        {
            get { return this._position; }
            set { this._position = value; }
        }

        public PvEEntity()
        {

        }
        public PvEEntity(uint uid)
        {
            this._uid = uid;
            this._position = new Position();
        }

        public PvEEntity(uint uid, Position spawn)
        {
            this._uid = uid;
            this._position = new Position();
            this._position = spawn;
        }

        public virtual uint OnTrigger(RoomSessionPlayer player)
        {
            return 0;
        }
    }
}
