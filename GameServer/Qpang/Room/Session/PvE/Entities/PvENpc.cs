using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class PvENpc : PvEEntity
    {
        private uint _id;
        private uint _health;

        public uint Id
        {
            get { return this._id; }
        }

        public uint Health
        {
            get { return this._health; }
        }


        public PvENpc(uint id, uint health, uint uid, Position spawn) : base(uid, spawn)
        {

        }
    }
}
