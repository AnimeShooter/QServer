using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class PvEObject : PvEEntity
    {
        private uint _id;
        private bool _activated = false;

        public uint Id
        {
            get { return this._id; }
        }

        public PvEObject(uint id, uint uid, Position spawn) : base(uid, spawn)
        {
            this._id = id;
        }

        public override uint OnTrigger(RoomSessionPlayer player)
        {
            // TODO do some more checks
            if (player == null || player.RoomSession == null)
                return 0;

            if (this._activated)
                return 0;

            // event or door?
            this._activated = true;
            player.RoomSession.RelayPlaying<GCPvEEventObject>(base.Uid, this._activated);

            return 0;
        }
    }
}
