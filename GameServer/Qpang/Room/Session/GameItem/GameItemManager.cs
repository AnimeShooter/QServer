﻿using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Util;

namespace Qserver.GameServer.Qpang
{
    public class GameItemManager
    {
        public enum Item : uint
        {
            AMMO_CLIP = 0x47000001,
            RED_MEDKIT = 0x47000002,
            EVENT_ITEM = 0x47000008,
            GREEN_MEDKIT = 0x4700000E,
            SKILL_CARD = 0x4700000F,
            GOLD_COIN = 0x47000010,
            SILVER_COIN = 0x47000011,
            BRONZE_COIN = 0x47000012
        };

        public static List<Item> PossibleItems = new List<Item>
        {
            Item.AMMO_CLIP,
            Item.RED_MEDKIT,
            Item.GREEN_MEDKIT,
            Item.SKILL_CARD,
        };

        public static Dictionary<Item, GameItem> MappedItems = new Dictionary<Item, GameItem>
        {
            { Item.RED_MEDKIT, new RedMedKit() },
            { Item.AMMO_CLIP, new AmmoClip() },
            { Item.GREEN_MEDKIT, new GreenMedKit() },
            { Item.SKILL_CARD, new SkillBook() },
            { Item.EVENT_ITEM, new EventItem() },
        };


        struct GameItemSpawn
        {
            public uint SpawnId;
            public uint ItemId;
            public uint LastPickUpTime;
            public Spawn Spawn;
        };

        public static Dictionary<uint, Spawn> MapBounts = new Dictionary<uint, Spawn>()
        {
            { 0, new Spawn(){ X=48, Y=0, Z=48} }, // Garden
            { 1, new Spawn(){ X=48, Y=0, Z=48} }, // Diorama
            { 2, new Spawn(){ X=64, Y=0, Z=64} }, // Skycastle
            { 3, new Spawn(){ X=34, Y=0, Z=34} }, // Ossyria
            { 4, new Spawn(){ X=62, Y=0, Z=18} }, // Dollhouse
            { 5, new Spawn(){ X=68, Y=0, Z=38} }, // City
            { 6, new Spawn(){ X=58, Y=0, Z=60} }, // Bunker
            { 7, new Spawn(){ X=58, Y=0, Z=58} }, // Temple
            { 8, new Spawn(){ X=37, Y=0, Z=92} }, // Brdige
            { 9, new Spawn(){ X=66, Y=0, Z=66} } // Castaway
        };

        private RoomSession _roomSession;
        private Dictionary<uint, GameItemSpawn> _items;
        private Dictionary<uint, bool> _eventItems;
        private bool _skillsEnabled;
        private bool _ready;
        private bool _eventItemsSpawed;
        private bool _isEventEligible;
        private uint _initTime;
        private uint _timeSinceEventItemSpawn;
        private Spawn _mapBounds;

        public static bool EventEnabled = false;
        //public static uint RespawnInterval = 15;
        public static uint RespawnInterval = 2; // TODO: 15
        public static uint EventItemBatchInterval = 200;

        public GameItemManager()
        {

        }

        public void Initialize(RoomSession roomSession)
        {
            this._roomSession = roomSession;
            if (MapBounts.ContainsKey(this._roomSession.Room.Map))
                this._mapBounds = MapBounts[this._roomSession.Room.Map];
            this._skillsEnabled = roomSession.Room.SkillsEnabled;
            this._isEventEligible = roomSession.Room.Password == "" && EventEnabled && MapBounts.ContainsKey(this._roomSession.Room.Map);

            this._items = new Dictionary<uint, GameItemSpawn>();
            this._eventItems = new Dictionary<uint, bool>();

            this._items.Add(0, new GameItemSpawn() { ItemId = 0, LastPickUpTime = 0 }); // empty 0 id

            var spawns = Game.Instance.SpawnManager.GetItemSpawns(this._roomSession.Room.Map);
            for (int i = 0; i < spawns.Count; i++)
            {
                this._items.Add((uint)(i+1), new GameItemSpawn()
                {
                    SpawnId = (uint)(i+1), // NOTE: id 0 is not accepted by gane?
                    ItemId = GetRandomItem(),
                    LastPickUpTime = 0,
                    Spawn = spawns[i]
                });
            }

            this._ready = true;
            this._initTime = Util.Util.Timestamp();

        }

        public void Tick()
        {
            if (!this._ready)
                return;

            var currTime = Util.Util.Timestamp();
            foreach (var item in this._items)
            {
                if (item.Value.LastPickUpTime == 0) // not looted
                    continue;
                else if (item.Value.LastPickUpTime + RespawnInterval < currTime) // looted, need respawn
                {
                    var spawnItem = new GameItemSpawn()
                    {
                        SpawnId = item.Value.SpawnId,
                        ItemId = GetRandomItem(),
                        LastPickUpTime = 0,
                        Spawn = item.Value.Spawn
                    };
                    this._items[item.Key] = spawnItem; // illegal?
                    var items = new List<GCGameItem.Item>()
                    {
                        new GCGameItem.Item()
                        {
                            ItemId = spawnItem.ItemId,
                            ItemUid = spawnItem.SpawnId,
                            X = spawnItem.Spawn.X,
                            Y = spawnItem.Spawn.Y,
                            Z = spawnItem.Spawn.Z
                        }
                    };
                    this._roomSession.RelayPlaying<GCGameItem>((byte)6, items, (uint)0); // TODO: Fix respawns
                }
            }

            // TODO: fix, experimental!
            // check for event batch spawn
            if (((!this._eventItemsSpawed && this._initTime + 60 < currTime) ||
                (this._eventItemsSpawed && this._timeSinceEventItemSpawn + EventItemBatchInterval < currTime)) && this._isEventEligible)
            {
                this._eventItemsSpawed = true;
                this._timeSinceEventItemSpawn = currTime;
                var items = new List<GCGameItem.Item>();
                var itemCount = this._roomSession.GetPlayingPlayers().Count * 2;

                if (itemCount > 30)
                    itemCount = 30; // prevent game from crashing ;D

                Random rnd = new Random();
                for (int i = 0; i < itemCount; i++)
                {
                    uint id = (uint)rnd.Next(5000, 0x00FFFFFF);
                    items.Add(new GCGameItem.Item()
                    {
                        ItemId = (uint)Item.EVENT_ITEM,
                        ItemUid = id,
                        X = (float)rnd.Next(0, 100) * this._mapBounds.X - this._mapBounds.X / 2,
                        Y = (float)rnd.Next(0, 100) * 400f,
                        Z = (float)rnd.Next(0, 100) * this._mapBounds.Z - this._mapBounds.Z / 2,
                    });
                    this._eventItems[id] = false;
                }
                this._roomSession.RelayPlaying<GCGameItem>((byte)6, items, (uint)0);

            }
        }

        public void SyncPlayer(RoomSessionPlayer player)
        {
            var items = new List<GCGameItem.Item>();
            foreach (var item in this._items)
                if (item.Value.ItemId != 0)
                    items.Add(new GCGameItem.Item()
                    {
                        ItemId = item.Value.ItemId,
                        ItemUid = item.Value.SpawnId,
                        X = item.Value.Spawn.X,
                        Y = item.Value.Spawn.Y,
                        Z = item.Value.Spawn.Z
                    });

            player.Post(new GCGameItem(6, items, 0));
        }

        public void Reset()
        {
            this._roomSession = null;
            this._items.Clear();
            this._eventItems.Clear();
        }

        public uint GetRandomItem()
        {
            // TODO: remove
            //return (uint)Item.SKILL_CARD;

            Random rnd = new Random();
            var index = rnd.Next(0, PossibleItems.Count);

            var item = PossibleItems[index];
            if(rnd.Next(0,100) <= 15)
            {
                if(this._roomSession.GameMode.IsTeamMode() && rnd.Next(0,100) < 33)
                        return (uint)Item.GREEN_MEDKIT;
                return (uint)Item.RED_MEDKIT;
            }
            
            // TODO: check for skillpoints and increase droprate based on skillpoint count

            if(this._roomSession.Room.SkillsEnabled && rnd.Next(0,100) < 50)
                return (uint)Item.SKILL_CARD;

            if (!this._roomSession.Room.MeleeOnly && rnd.Next(0, 100) < 90)
                return (uint)Item.AMMO_CLIP;

            return (uint)Item.RED_MEDKIT; // worst case secanrio
        }

        public void OnPickUp(RoomSessionPlayer player, PvEItem item)
        {
            // TODO handle loot?

            this._roomSession.RelayPlaying<GCGameItem>((byte)1, player.Player.PlayerId, item.ItemId, item.Uid, (uint)0);
        }

        public void OnPickUp(RoomSessionPlayer player, uint spawnId)
        {
            if (this._items == null)
                return;

            // try event item
            if (!this._items.ContainsKey(spawnId))
            {
                OnPickupEventItem(player, spawnId);
                return;
            }

            // item ready check
            var item = this._items[spawnId];
            if (item.ItemId == 0)
                return;

            // distance check
            if (!player.IsInRange(item.Spawn, 1.75f, false))
                return;

            // break skills if needed
            var activeSkill = player.SkillManager.ActiveSkillCard;
            if (activeSkill != null)
            {
                switch ((Items)activeSkill.Id)
                {
                    case Items.SKILL_CAMO:
                        player.SkillManager.DisableSkill(); // consume but allow skill usage
                        break;
                    case Items.SKILL_IRONWALL:
                    case Items.SKILL_IRONWALL2:
                    default:
                        return; // prevent skill usage
                }
            }

            // check if prey
            bool isPrey = player.RoomSession.Room.Mode == GameMode.Mode.PREY && player.RoomSession.PublicEnemy == player;

            // check whitelisted object
            if (item.ItemId == (uint)Item.RED_MEDKIT || item.ItemId == (uint)Item.GREEN_MEDKIT || item.ItemId == (uint)Item.AMMO_CLIP
                || item.ItemId == (uint)Item.SKILL_CARD)
            {

                // NOTE: Only allow Prey to loot loot ammo, all other loot will remove
                uint identifier = 0;
                if (!isPrey || isPrey && item.ItemId == (uint)Item.AMMO_CLIP)
                    identifier = MappedItems[(Item)item.ItemId].OnPickUp(player);

                this._roomSession.RelayPlaying<GCGameItem>((byte)1, player.Player.PlayerId, item.ItemId, item.SpawnId, identifier);

                this._items[spawnId] = new GameItemSpawn()
                {
                    ItemId = 0,
                    SpawnId = item.SpawnId,
                    LastPickUpTime = Util.Util.Timestamp(),
                    Spawn = item.Spawn
                };
            }
        }

        public void OnPickupEventItem(RoomSessionPlayer player, uint id)
        {
            if(!this._eventItems.ContainsKey(id))
                return;

            if (this._eventItems[id])
                return;

            uint identifier = MappedItems[Item.EVENT_ITEM].OnPickUp(player);
            this._roomSession.RelayPlaying<GCGameItem>((byte)1, player.Player.PlayerId, (uint)Item.EVENT_ITEM, id, identifier);
        }

        private void SpawnItem(GameItemSpawn item)
        {
            List<GCGameItem.Item> items = new List<GCGameItem.Item>()
            {
                new GCGameItem.Item()
                {
                    ItemId = item.ItemId,
                    ItemUid = item.SpawnId,
                    X = item.Spawn.X,
                    Y = item.Spawn.Y,
                    Z = item.Spawn.Z,
                }
            };

            this._roomSession.RelayPlaying<GCGameItem>((byte)6, items, (uint)0);
        }
    }
}
