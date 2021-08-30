using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class PvEAreaTrigger : PvEEntity
    {
        private uint _id;

        public uint Id
        {
            get { return this._id; }
            // set { this._id = Id; }
        }

        public PvEAreaTrigger(uint id, uint uid, Position spawn) : base(uid, spawn)
        {
            this._id = id;
        }
    }
}
