using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class PvEItem : PvEEntity
    {
        private uint _itemId;

        public uint ItemId
        {
            get { return this._itemId; }
           // set { this._itemId = value; } 
        }

        public PvEItem(uint itemId, uint uid, Position spawn) : base (uid, spawn)
        {
            this._itemId = itemId;
        }

        public override uint OnTrigger(RoomSessionPlayer player)
        {
            // TODO do some more checks
            if (player == null || player.RoomSession == null)
                return 0;

            player.RoomSession.ItemManager.OnPickUp(player, this);

            return 0;
        }


    }
}
