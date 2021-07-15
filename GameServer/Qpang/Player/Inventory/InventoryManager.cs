using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Network.Managers;

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

            this._cards.Add(1, new InventoryCard()
            {
                Id = 1,
                BoostLevel = 2,
                IsActive = false,
                IsGiftable = false,
                IsOpened = true,
                ItemId = 1095368711,
                Period = 2,
                PeriodeType = 2


            });
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
            if (!HasCard(cardId))
                return;

            lock(this._lock)
            {
                if (this._player == null)
                    return;

                bool isEquiped = this._player.EquipmentManager.HasEquipped(cardId);
                if (isEquiped)
                    return;

                this._cards.Remove(cardId);

                // TODO: remove from database!

                this._player.SendLobby(LobbyManager.Instance.RemoveCard(cardId));
            }

            return;
        }

        public void SetCardActive(ulong cardId, bool isActive)
        {
            lock(this._lock)
            {
                if (!this._cards.ContainsKey(cardId))
                    return;

                var card = this._cards[cardId];
                var duplicate = this._player.EquipmentManager.HasFunctionCard(card.ItemId);

                if(isActive && !duplicate)
                {
                    card.TimeCreated = DateTime.UtcNow;
                    card.IsActive = true;
                    this._player.EquipmentManager.AddFunctionCard(cardId);
                    this._player.SendLobby(LobbyManager.Instance.EnabledFunctionCard(card));
                }else if(!isActive && duplicate)
                {
                    card.IsActive = true;
                    this._player.EquipmentManager.AddFunctionCard(cardId);
                    this._player.SendLobby(LobbyManager.Instance.EnabledFunctionCard(card));
                }

                this._cards[card.Id] = card;
            }
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
