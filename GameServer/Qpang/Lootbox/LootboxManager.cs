using Qserver.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class LootboxManager
    {
        private List<LootboxLoot> _lootTable;

        public LootboxManager ()
        {
            this._lootTable = new List<LootboxLoot>();
            // TODO db
            Log.Message(LogType.MISC, $"LootboxManager loaded {this._lootTable.Count} Lootbox Loot Tables from the database!");
        }

        public InventoryCard LootLootbox()
        {
            // TODO
            var loot = new InventoryCard()
            {

            };

            return loot;
        }
    }
}
