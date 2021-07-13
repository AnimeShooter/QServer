using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class InventoryManager
    {
        private Player _player;
        private object _lock;

        private Dictionary<ulong, InventoryCard> _cards;
        private Dictionary<ulong, InventoryCard> _gifts;

        public InventoryManager(Player player)
        {
            this._player = player;
            this._lock = new object();

            this._cards = new Dictionary<ulong, InventoryCard>();
            this._gifts = new Dictionary<ulong, InventoryCard>();
            // TODO: Database
        }

        public List<InventoryCard> List()
        {
            lock(this._lock)
            {
                List<InventoryCard> cards = new List<InventoryCard>();
                foreach(var c in this._cards)
                {
                    cards.Add(c.Value);
                }
                return cards;
            }
        }

        public List<InventoryCard> ListGifts()
        {
            lock (this._lock)
            {
                List<InventoryCard> cards = new List<InventoryCard>();
                foreach (var c in this._gifts)
                {
                    cards.Add(c.Value);
                }
                return cards;
            }
        }

        public InventoryCard Get(ulong cardId)
        {
            lock(this._lock)
                if (this._cards.ContainsKey(cardId))
                    return this._cards[cardId];
            return new InventoryCard();
        }

        public bool HasCard(ulong cardId)
        {
            lock (this._lock)
                return this._cards.ContainsKey(cardId);
        }

        public void DeleteCard(ulong cardId)
        {
            // TODO
            return;
        }

        public void SetCardActive(ulong cardId, bool isActive)
        {
            // TODO:
            return;
        }

        public bool AddCard(InventoryCard card)
        {
            if (!HasSpace())
                return false;

            lock (this._lock)
                this._cards[card.Id] = card;

            return true;
        }

        public void StoreCard(InventoryCard card)
        {

        }

        public void UseCard(uint cardId, uint playtime)
        {

        }

        public bool IsExpired(ulong cardId)
        {
            return false;
        }

        public bool HasSpace()
        {
            return this._cards.Count + this._gifts.Count < 200;
        }

        public void GiftCard(InventoryCard card, Player player)
        {

        }

        public void ReceiveGift(InventoryCard card, string sender)
        {

        }

        public void OpenGift(ulong cardId)
        {

        }
        public bool HasGiftSpace()
        {
            return this._gifts.Count < 5;
        }
        public void Close()
        {
            // TODO
        }
    }
}
