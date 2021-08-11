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

        private Dictionary<uint, uint> _traders;
        private Dictionary<uint, uint> _pending;
        private Dictionary<uint, List<InventoryCard>> _items;
        private object _lock;

        public TradeManager()
        {
            this._traders = new Dictionary<uint, uint>();
            this._pending = new Dictionary<uint, uint>();
            this._items = new Dictionary<uint, List<InventoryCard>>();
            this._lock = new object();
        }

        public bool OnRequest(Player player, uint targetId)
        {
            if (player == null)
                return false;

            lock(this._lock)
                lock(player.Lock)
                {
                    if (this._pending.ContainsKey(targetId))
                        return false; // do not alow trading while already requestion a trade?
                    this._pending.Add(player.PlayerId, targetId);

                    this._items.Add(player.PlayerId, new List<InventoryCard>());
                    this._items.Add(targetId, new List<InventoryCard>());
                }

            return true;
        }

        public void OnCancel(Player player)
        {
            if (player == null)
                return;

            lock(this._lock)
                lock(player.Lock)
                {
                    uint targetId = 0;
                    if (this._items.ContainsKey(player.PlayerId))
                    {
                        // remove traders
                        if (this._pending.ContainsKey(player.PlayerId))
                        {
                            targetId = this._pending[player.PlayerId];
                            this._pending.Remove(player.PlayerId);
                        }
                        if (this._traders.ContainsKey(player.PlayerId))
                        {
                            targetId = this._traders[player.PlayerId];
                            this._traders.Remove(player.PlayerId);
                        }

                        // remove items
                        this._items.Remove(player.PlayerId);
                    }
                }
        }

        public void OnComplete(Player player)
        {
            lock (this._lock)
                lock (player.Lock)
                {
                    if (!this._traders.ContainsKey(player.PlayerId))
                        return;

                    var targetId = this._traders[player.PlayerId];
                    var target = Game.Instance.GetPlayer(targetId);

                    // grab items from trade
                    List<InventoryCard> playerStach = null;
                    List<InventoryCard> targetStach = null;

                    if (this._items.ContainsKey(player.PlayerId))
                    {
                        playerStach = this._items[player.PlayerId];
                        this._items.Remove(player.PlayerId);
                    }
                    if (this._items.ContainsKey(player.PlayerId))
                    {
                        targetStach = this._items[targetId];
                        this._items.Remove(targetId);
                    }

                    // Make sure nothing got fucked up for some  reason
                    if (playerStach == null || targetStach == null)
                        return;

                    // transfer
                    lock(target.Lock)
                    {

                    }

                    this._traders.Remove(player.PlayerId);
                }
        }

        //

        // TODO: keep track of items in trade?

        public Player FindTradingBuddy(Player player)
        {
            // NOTE: do different event based on, pending or already in trade?
            lock (this._lock)
                lock (player.Lock)
                {
                    if (this._pending.ContainsKey(player.PlayerId))
                        return Game.Instance.GetPlayer(this._pending[player.PlayerId]);

                    if (this._traders.ContainsKey(player.PlayerId))
                        return Game.Instance.GetPlayer(this._traders[player.PlayerId]);
                }

            return null;
        }

    }
}
