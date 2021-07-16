using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Managers;
using Qserver.Util;

namespace Qserver.GameServer.Qpang
{
    public class ShopManager
    {
        private Dictionary<uint, ShopItem> _items;
        private List<ShopItem> _orderedItems;

        public ShopManager()
        {
            this._items = new Dictionary<uint, ShopItem>();
            this._orderedItems = new List<ShopItem>();

            // TODO: database
            var dbitems = Game.Instance.ItemsRepository.GetItems().Result;
            foreach(var dbitem in dbitems)
            {
                this._items.Add(dbitem.seq_id, new ShopItem()
                {
                    SeqId = dbitem.seq_id,
                    ItemId = dbitem.item_id,
                    IsCash = dbitem.pay_type == 1,
                    Price = dbitem.price,
                    SoldCount = dbitem.sold_count,
                    Stock = dbitem.stock,
                    ShopCategory = dbitem.status,
                    Type = dbitem.type,
                    PeriodType = dbitem.use_up,
                    Period = dbitem.period,
                    MinLevel = dbitem.level
                });
            }
            Log.Message(LogType.MISC, $"ShopManager loaded {this._items.Count} Items from the database!");
        }

        public List<ShopItem> List()
        {
            List<ShopItem> items = new List<ShopItem>();
            foreach (var item in this._items)
                items.Add(item.Value);
            return items;
        }

        public bool Exists(uint seqId)
        {
            return this._items.ContainsKey(seqId);
        }

        public ShopItem Get(uint seqId)
        {
            if (this._items.ContainsKey(seqId))
                return this._items[seqId];
            return new ShopItem();
        }

        public void Buy(Player player, uint seqId)
        {
            // TODO: database
            if (player == null)
                return;
            
            if (!Exists(seqId))
                return;

            if (!player.InventoryManager.HasSpace())
                return;

            var shopItem = Get(seqId);

            if (shopItem.SoldCount >= shopItem.Stock && shopItem.Stock != 9999)
                return;

            var money = shopItem.IsCash ? player.Cash : player.Don;
            var canBuy = money >= shopItem.Price;

            if (!canBuy)
                return;

            var card = InventoryCard.FromShopItem(shopItem);

            // TODO: database
            card.Id = 2; // TODO: get from DB!

            card.PlayerOwnedId = player.PlayerId;
            if (shopItem.IsCash)
                player.RemoveCash(shopItem.Price);
            else
                player.RemoveDon(shopItem.Price);

            player.InventoryManager.AddCard(card);
            player.SendLobby(LobbyManager.Instance.ShopItems(List()));
            player.SendLobby(LobbyManager.Instance.CardPurchaseComplete(shopItem, new List<InventoryCard>() { card }, shopItem.IsCash ? player.Cash : player.Don));
        }
    }
}
