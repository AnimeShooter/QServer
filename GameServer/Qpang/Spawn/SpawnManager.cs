using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Util;

namespace Qserver.GameServer.Qpang
{
    public class SpawnManager
    {
        private Dictionary<byte, Dictionary<byte, List<Spawn>>> _spawns;
        private Dictionary<byte, List<Spawn>> _itemSpawns;
        private object _lock;

        public SpawnManager()
        {
            this._lock = new object();
            this._spawns = new Dictionary<byte, Dictionary<byte, List<Spawn>>>();
            this._itemSpawns = new Dictionary<byte, List<Spawn>>();

            var mapspawns = Game.Instance.SpawnsRepository.GetSpawns().Result;
            foreach(var ms in mapspawns)
            {
                Spawn s = new Spawn()
                {
                    X = ms.x,
                    Y = ms.y,
                    Z = ms.z
                };
                
                if (!this._spawns.ContainsKey(ms.map_id))
                    this._spawns.Add(ms.map_id, new Dictionary<byte, List<Spawn>>());

                if (!this._spawns[ms.map_id].ContainsKey(ms.team))
                    this._spawns[ms.map_id].Add(ms.team, new List<Spawn>());

                this._spawns[ms.map_id][ms.team].Add(s);
            }
            Log.Message(LogType.MISC, $"SpawnManager loaded {mapspawns.Count} Spawns from the database!");

            var itemspawns = Game.Instance.SpawnsRepository.GetGameItemSpawns().Result;
            foreach(var itemspawn in itemspawns)
            {
                Spawn s = new Spawn()
                {
                    X = itemspawn.x,
                    Y = itemspawn.y,
                    Z = itemspawn.z,
                };

                if (!this._itemSpawns.ContainsKey(itemspawn.map_id))
                    this._itemSpawns.Add(itemspawn.map_id, new List<Spawn>());

                this._itemSpawns[itemspawn.map_id].Add(s);
            }
            Log.Message(LogType.MISC, $"SpawnManager loaded {itemspawns.Count} Item Spawns from the database!");
        }

        public Spawn GetRandomSpawn(byte map, byte team)
        {
            lock(this._lock)
            {
                if (team != 0 && team != 1 && team != 2)
                    return new Spawn();

                var spawns = this._spawns[map][team];
                if (spawns.Count == 0)
                    return new Spawn();

                Random rnd = new Random();
                return spawns[rnd.Next(0, spawns.Count)];
            }
        }

        public Spawn GetRandomTeleportSpawn(byte map)
        {
            lock (this._lock)
            {
                List<Spawn> spawns = null;
                if (this._spawns[map].ContainsKey(98))
                    spawns = this._spawns[map][98];

                if (spawns == null || spawns.Count == 0)
                    return new Spawn()
                    {
                        X = 0xFF,
                        Y = 0xFF,
                        Z = 0xFF
                    };

                Random rnd = new Random();
                return spawns[rnd.Next(0, spawns.Count)];
            }
        }

        public List<Spawn> GetItemSpawns(byte map)
        {
            lock (this._lock)
            {
                if (!this._itemSpawns.ContainsKey(map))
                    return new List<Spawn>();
                    //{ 
                    //    new Spawn(){ X = -200, Y = -100, Z = 10 } 
                    //};
                return this._itemSpawns[map];
            }
                
        }

        public Spawn GetEssenceSpawn(byte map)
        {
            lock (this._lock)
            {
                List<Spawn> spawns = null;
                if (this._spawns[map].ContainsKey(99))
                    spawns = this._spawns[map][99];

                if (spawns == null)
                    return new Spawn();

                return spawns[0];
            }
        }
    }
}
