using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class TradeManager
    {
        // - keep track of who is trading
        // - keep track of trade status
        // - exchange items

        private Dictionary<uint, uint> _accepted;
        private Dictionary<uint, uint> _pending;
        private Dictionary<uint, uint> _traders;
        private Dictionary<uint, uint> _request;
        
        private Dictionary<uint, Dictionary<ulong, InventoryCard>> _items;
        private object _lock;

        public TradeManager()
        {
            this._accepted = new Dictionary<uint, uint>();
            this._pending = new Dictionary<uint, uint>();
            this._traders = new Dictionary<uint, uint>();
            this._request = new Dictionary<uint, uint>();
            this._items = new Dictionary<uint, Dictionary<ulong, InventoryCard>>();
            this._lock = new object();
        }

        // Trade if possible
        public bool CanTrade(Player player)
        {
            lock(this._lock)
                return this._accepted.ContainsKey(player.PlayerId) &&
                    this._accepted.ContainsKey(this._accepted[player.PlayerId]);
        }

        public bool CompleteTrade(Player player)
        {
            lock(this._lock)
            {
                // Both parties have agreed, do trade
                if (!this._accepted.ContainsKey(player.PlayerId))
                    return false;

                var targetId = this._accepted[player.PlayerId];
                var target = Game.Instance.GetPlayer(targetId);

                // grab items from trade
                Dictionary<ulong, InventoryCard> playerStash = null;
                Dictionary<ulong, InventoryCard> targetStash = null;

                // remove items data
                if (this._items.ContainsKey(player.PlayerId))
                {
                    playerStash = this._items[player.PlayerId];
                    this._items.Remove(player.PlayerId);
                }
                if (this._items.ContainsKey(targetId))
                {
                    targetStash = this._items[targetId];
                    this._items.Remove(targetId);
                }

                // Make sure nothing got fucked up for some  reason
                if (playerStash == null || targetStash == null)
                    return false;

                // Transfer player
                foreach (var item in playerStash.Values)
                    player.InventoryManager.TradeItem(item.Id, targetId);

                // Transfer target
                foreach (var item in targetStash.Values)
                    target.InventoryManager.TradeItem(item.Id, player.PlayerId);

                // remove trade data
                this._accepted.Remove(player.PlayerId);
                this._accepted.Remove(targetId);
            }
            return true;
        }

        // Propose accept
        public bool OnProposalAccept(Player player)
        {
            lock(this._lock)
            {
                if(this._pending.ContainsKey(player.PlayerId))
                {
                    this._accepted.Add(player.PlayerId, this._pending[player.PlayerId]);
                    this._pending.Remove(player.PlayerId);
                }
            }

            // trade if possiblr
            if(CanTrade(player))
                return CompleteTrade(player);

            return true;
        }

        // trade => Pending/waiting for approval
        public bool OnTradePropose(Player player)
        {
            lock (this._lock)
            {
                if (this._traders.ContainsKey(player.PlayerId))
                {
                    this._pending.Add(player.PlayerId, this._traders[player.PlayerId]);
                    this._traders.Remove(player.PlayerId);
                }
                else
                    return false;
            }
            return true;
        }

        // Request => trade
        public bool OnRequestAccept(Player player)
        {
            lock (this._lock)
            {
                // copy from pending to trading
                uint targetId = 0;
                if (this._request.ContainsKey(player.PlayerId))
                {
                    targetId = this._request[player.PlayerId];
                    this._traders.Add(player.PlayerId, targetId);
                    this._request.Remove(player.PlayerId);
                }
                if (this._request.ContainsKey(targetId))
                {
                    this._traders.Add(targetId, this._request[targetId]);
                    this._request.Remove(targetId);
                }

                // This should not yet be possible?
                if (this._traders.ContainsKey(player.PlayerId) || this._traders.ContainsKey(targetId))
                    return false;

                this._traders.Add(player.PlayerId, targetId);
                this._traders.Add(targetId, player.PlayerId);

            }
            return true;
        }

        // add Request
        public bool OnTradeRequest(Player player, uint targetId)
        {
            if (player == null)
                return false;

            lock(this._lock)
            {
                if (this._request.ContainsKey(targetId))
                    return false; // do not alow trading while already requestion a trade?
                this._request.Add(player.PlayerId, targetId);
                this._request.Add(targetId, player.PlayerId);

                this._items.Add(player.PlayerId, new Dictionary<ulong, InventoryCard>());
                this._items.Add(targetId, new Dictionary<ulong, InventoryCard>());
            }

            return true;
        }

        public void OnCancel(Player player)
        {
            if (player == null)
                return;

            lock (this._lock)
            {
                uint targetId = 0;
                if (this._items.ContainsKey(player.PlayerId))
                {
                    // remove traders
                    if (this._request.ContainsKey(player.PlayerId))
                    {
                        targetId = this._request[player.PlayerId];
                        this._request.Remove(player.PlayerId);
                    }
                    if (this._traders.ContainsKey(player.PlayerId))
                    {
                        targetId = this._traders[player.PlayerId];
                        this._traders.Remove(player.PlayerId);
                    }
                    if (this._pending.ContainsKey(player.PlayerId))
                    {
                        targetId = this._pending[player.PlayerId];
                        this._pending.Remove(player.PlayerId);
                    }
                    if (this._accepted.ContainsKey(player.PlayerId))
                    {
                        targetId = this._accepted[player.PlayerId];
                        this._accepted.Remove(player.PlayerId);
                    }

                    if (this._request.ContainsKey(targetId))
                        this._request.Remove(targetId);

                    if (this._traders.ContainsKey(targetId))
                        this._traders.Remove(targetId);

                    if (this._pending.ContainsKey(targetId))
                        this._pending.Remove(targetId);

                    if (this._accepted.ContainsKey(targetId))
                        this._accepted.Remove(targetId);

                    // remove items
                    this._items.Remove(player.PlayerId);
                    this._items.Remove(targetId);
                }
            }
        }

        public bool AddItem(Player player, InventoryCard card)
        {
            if (!player.InventoryManager.HasCard(card.Id))
                return false; // no cheating

            // TODO: obtain server card pre-call?
            var realCard = player.InventoryManager.Get(card.Id);
            if (!realCard.IsGiftable)
                return false; // must be tradable

            lock (this._lock)
            {
                if (!this._items.ContainsKey(player.PlayerId))
                    return false; // unk error

                var stash = this._items[player.PlayerId];
                if (stash.ContainsKey(card.Id))
                    return false; // duplicate card

                stash.Add(card.Id, card);                    
            }
            return true;
        }

        public bool RemoveItem(Player player, ulong cardid)
        {
            lock (this._lock)
            {
                if (!this._items.ContainsKey(player.PlayerId))
                    return false; // unk error

                var stash = this._items[player.PlayerId];
                stash.Remove(cardid);
            }
            return true;
        }

        public Player FindTradingBuddy(Player player)
        {
            // NOTE: do different event based on, pending or already in trade?
            lock (this._lock)
            {
                if (this._request.ContainsKey(player.PlayerId))
                    return Game.Instance.GetPlayer(this._request[player.PlayerId]);

                if (this._traders.ContainsKey(player.PlayerId))
                    return Game.Instance.GetPlayer(this._traders[player.PlayerId]);

                if (this._pending.ContainsKey(player.PlayerId))
                    return Game.Instance.GetPlayer(this._pending[player.PlayerId]);

                if (this._accepted.ContainsKey(player.PlayerId))
                    return Game.Instance.GetPlayer(this._accepted[player.PlayerId]);
            }

            return null;
        }

    }
}
