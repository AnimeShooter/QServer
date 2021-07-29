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

            var dbitems = Game.Instance.ItemsRepository.GetInventoryCards(this._player.PlayerId).Result;
            foreach(var dbitem in dbitems)
            {
                var card = new InventoryCard()
                {
                    Id = dbitem.id,
                    PlayerOwnedId = dbitem.player_id,
                    ItemId = dbitem.item_id,
                    Type = dbitem.type,
                    PeriodeType = (byte)dbitem.period_type,
                    Period = dbitem.period,
                    IsActive = dbitem.active == 1,
                    IsOpened = dbitem.opened == 1,
                    IsGiftable = dbitem.giftable == 1,
                    BoostLevel = dbitem.boost_level,
                    TimeCreated = dbitem.time
                };
                if (card.IsOpened)
                    this._cards[card.Id] = card;
                else
                    this._gifts[card.Id] = card;
            }
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

            if (this._player.TestRealm)
                return; // no deleting in test realm

            lock (this._player.Lock)
            {
                if (this._player == null)
                    return;

                bool isEquiped = this._player.EquipmentManager.HasEquipped(cardId);
                if (isEquiped)
                    return;

                this._cards.Remove(cardId);

                Game.Instance.ItemsRepository.DeleteCard(this._player.PlayerId, cardId).GetAwaiter().GetResult();

                this._player.SendLobby(LobbyManager.Instance.RemoveCard(cardId));
            }

            return;
        }

        public void SetCardActive(ulong cardId, bool isActive)
        {
            lock (this._lock)
            {
                if (!this._cards.ContainsKey(cardId))
                    return;

                lock (this._player.Lock)
                {
                    var card = this._cards[cardId];
                    var alreadyEquipped = this._player.EquipmentManager.HasFunctionCard(card.ItemId);

                    if (isActive && !alreadyEquipped)
                    {
                        card.TimeCreated = Util.Util.Timestamp();
                        card.IsActive = true;
                        this._player.EquipmentManager.AddFunctionCard(cardId);
                        this._player.SendLobby(LobbyManager.Instance.EnabledFunctionCard(card));
                    } else if (!isActive && alreadyEquipped)
                    {
                        card.IsActive = false;
                        this._player.EquipmentManager.RemoveFunctionCard(cardId);
                        this._player.SendLobby(LobbyManager.Instance.DisableFunctionCard(card));
                    }
                    this._cards[card.Id] = card;
                }
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
            lock(this._lock)
            {
                if (this._player == null)
                    return;

                lock(this._player.Lock)
                {
                    // db register purchase
                    card.Id = Game.Instance.ItemsRepository.CreateItem(card, this._player).Result;

                    card.TimeCreated = Util.Util.Timestamp();
                    card.PlayerOwnedId = this._player.PlayerId;

                    AddCard(card);
                }
            }
        }

        public void UseCard(ulong cardId, uint playtime)
        {
            lock (this._lock)
            {
                if (this._player == null)
                    return;

                if (this._player.TestRealm)
                    return; // no consuming cards on test realm

                if (!this._cards.ContainsKey(cardId))
                    return;

                lock(this._player.Lock)
                {
                    var card = this._cards[cardId];

                    if (card.PeriodeType == 254)
                        return; // Unlimited

                    if (card.PeriodeType == 3) // rounds
                    {
                        if (card.Period > 0)
                            card.Period--;
                    }
                    else if (card.PeriodeType == 2) // time based?
                        card.Period = card.Period <= playtime ? (ushort)0 : (ushort)(card.Period - playtime);

                    if (card.PeriodeType != 254)
                    {
                        Game.Instance.ItemsRepository.UserCard(playtime, cardId).GetAwaiter().GetResult();
                    }

                    if (card.Period == 0)
                    {
                        // expired?
                        if (card.Type == 86 || card.Type == 87)
                            this._player.EquipmentManager.UnequipItem(cardId);
                        else if (card.Type == 70)
                        {
                            card.IsActive = false;
                            this._player.EquipmentManager.RemoveFunctionCard(cardId);
                        }
                    }
                }
            }
        }

        public bool IsExpired(ulong cardId)
        {
            lock(this._lock)
            {
                if (!this._cards.ContainsKey(cardId))
                    return true;

                return this._cards[cardId].Period == 0;
            }
        }

        public bool HasSpace()
        {
            return this._cards.Count + this._gifts.Count < 200;
        }

        public void GiftCard(InventoryCard card, Player target)
        {
            lock (this._player.Lock)
            {
                if (this._player == null)
                    return;

                if (!this._cards.ContainsKey(card.Id))
                    return;

                if (target.TestRealm)
                    return; // no losing/duping cards

                this._cards.Remove(card.Id);
                card.TimeCreated = Util.Util.Timestamp();
                target.InventoryManager.ReceiveGift(card, this._player.Name);

                this._player.SendLobby(LobbyManager.Instance.GiftCardSuccess(card.Id));
                this._player.EquipmentManager.Save();
            }
        }

        public void ReceiveGift(InventoryCard card, string sender)
        {
            lock(this._lock)
            {
                if (this._player == null)
                    return;

                if (this._player.TestRealm)
                    return;

                lock (this._player.Lock)
                {
                    card.IsOpened = false;
                    card.PlayerOwnedId = this._player.PlayerId;
                    this._gifts[card.Id] = card;

                    this._player.SendLobby(LobbyManager.Instance.ReceiveGift(card, sender));
                }
            }
        }

        public void OpenGift(ulong cardId)
        {
            lock(this._lock)
            {
                if (this._player == null)
                    return;

                if (this._player.TestRealm)
                    return;

                if (!this._gifts.ContainsKey(cardId))
                    return;

                lock (this._player.Lock)
                {
                    var card = this._gifts[cardId];
                    card.IsOpened = true;

                    this._cards[card.Id] = card; // add to cards
                    this._gifts.Remove(cardId); // rm from gifts

                    Game.Instance.ItemsRepository.OpenCardGift(cardId).GetAwaiter().GetResult();

                    this._player.SendLobby(LobbyManager.Instance.OpenGiftSuccess(this._player, card));
                }  
            }
        }

        public bool HasGiftSpace()
        {
            return this._gifts.Count < 5;
        }
        public void Close()
        {
            if (this._player.TestRealm)
                return;

            lock(this._lock)
            {
                foreach (var card in this._cards)
                    if (ItemID.IsEquippableFunction(card.Value.ItemId))
                        Game.Instance.ItemsRepository.SetCardActive(card.Key, card.Value.IsActive).GetAwaiter().GetResult();
            }
        }
    }
}
