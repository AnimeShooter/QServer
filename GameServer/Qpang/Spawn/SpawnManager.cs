using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Spawn
{
    public class SpawnManager
    {
        private Dictionary<byte, Dictionary<byte, List<Spawn>>> _spawns;
        private List<Spawn> _itemSpawns;
        private object _lock;

        public void Initialize()
        {
            this._lock = new object();

            // Database.PlayerSpawns

            this._spawns.Clear();
            this._itemSpawns.Clear();

            // init database data
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
                var spawns = this._spawns[map][98];
                if (spawns.Count == 0)
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

        public Spawn GetIremSpawns(byte map)
        {
            lock (this._lock)
            {
                return this._itemSpawns[map];
            }
        }

        public Spawn getEssenceSpawn(byte map)
        {
            lock (this._lock)
            {
                var spawns = this._spawns[map][99];
                if (spawns.Count == 0)
                    return new Spawn();

                return spawns[0];
            }
        }
    }
}
