using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Database;
using Qserver.Database.Repositories;
using Qserver.Util;

namespace Qserver.GameServer.Qpang
{
    public class CraneManager
    {
        private object _lock;
        private List<DBCraneItem> _items;

        public bool Enabled
        {
            get { return _items.Count > 0; }
        }

        public CraneManager()
        {
            this._lock = new object();

            this._items = Game.Instance.CraneRepository.GetCraneItems().Result;
            Log.Message(LogType.MISC, $"CraneManager loaded {this._items.Count} Items from the database!");
        }

        public InventoryCard GetRandomItem()
        {
            lock(this._lock)
            {
                Random rnd = new Random();
                var index = rnd.Next(0, this._items.Count); // TODO: fix RNG???
                var item = this._items[index];

                InventoryCard card = new InventoryCard();
                card.ItemId = item.item_id;
                card.Type = item.item_type;
                card.IsGiftable = true;
                card.IsOpened = true;
                card.IsActive = false;
                card.BoostLevel = 0;
                card.TimeCreated = Util.Util.Timestamp();

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
