using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Database;
using Qserver.GameServer.Database.Repositories;
using Qserver.Util;

namespace Qserver.GameServer.Qpang
{
    public class CraneManager
    {
        private object _lock;
        private List<CraneItem> _items;
        private CraneRepository _craneRepository;

        public bool Enabled
        {
            get { return _items.Count > 0; }
        }

        public CraneManager()
        {
            this._lock = new object();

            Log.Message(LogType.MISC, "Loading Crane Items from database...");
            this._craneRepository = new CraneRepository(DatabaseManager.MySqlFactory);
            this._items = this._craneRepository.GetCraneItems().Result;
            Log.Message(LogType.MISC, $"{this._items.Count} Crane Items have been loaded from the database!");

        }

        public InventoryCard GetRandomItem()
        {
            lock(this._lock)
            {
                Random rnd = new Random();
                var item = this._items[rnd.Next(0, this._items.Count)];

                InventoryCard card = new InventoryCard();
                card.ItemId = item.item_id;
                card.Type = item.item_type;
                card.IsGiftable = true;

                bool IsUnlimited = (rnd.Next(0, 1000) % 100 == 0); // 1% 

                if(IsUnlimited)
                {
                    card.PeriodeType = 254;
                    card.Period = 1;
                }else
                {
                    card.PeriodeType = 3;
                    card.Period = (ushort)(rnd.Next(0, item.period / 2) + item.period);
                }
                return card;
            }
        }

    }
}
