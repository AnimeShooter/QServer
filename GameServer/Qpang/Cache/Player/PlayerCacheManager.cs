using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class PlayerCacheManager
    {
        private object _lock;

        private Dictionary<uint, Player> _playersById;
        private Dictionary<string, Player> _playersByName;


        public PlayerCacheManager()
        {
            this._lock = new object();
            this._playersById = new Dictionary<uint, Player>();
            this._playersByName = new Dictionary<string, Player>();
        }

        public void Invalidate(uint id)
        {
            var player = Get(id);
            if (player == null)
                return;

            lock(this._lock)
            {
                this._playersById.Remove(id);
                this._playersByName.Remove(player.Name);
            }
        }

        public Player Cache(uint id)
        {
            Player player = new Player(id);
            lock (this._lock)
            {
                if (this._playersById.ContainsKey(id))
                    this._playersById[id] = player;
                else
                    this._playersById.Add(id, player);
            }
            return player;
        }

        public Player Cache(string name)
        {
            uint playerId = Game.Instance.PlayersRepository.GetPlayerId(name).Result;
            if (playerId == 0)
                return null;

            return Cache(playerId);
        }

        public Player Get(uint id)
        {
            lock(this._lock)
            {
                if (this._playersById.ContainsKey(id))
                    return this._playersById[id];
                return null;
            }
        }

        public Player Get(string name)
        {
            lock (this._lock)
            {
                if (this._playersByName.ContainsKey(name))
                    return this._playersByName[name];
                return null;
            }
        }

        public void Cache(Player player)
        {
            if (player == null)
                return;

            lock(this._lock)
            {
                if (this._playersById.ContainsKey(player.PlayerId))
                    this._playersById[player.PlayerId] = player;
                else
                    this._playersById.Add(player.PlayerId, player);

                if (this._playersByName.ContainsKey(player.Name))
                    this._playersByName[player.Name] = player;
                else
                    this._playersByName.Add(player.Name, player);
            }
        }

        public void Clear()
        {
            lock(this._lock)
            {
                this._playersById.Clear();
                this._playersByName.Clear();
            }
        }
    }
}
