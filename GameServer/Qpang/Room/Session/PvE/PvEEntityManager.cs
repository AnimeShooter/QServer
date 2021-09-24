using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class PvEEntityManager
    {
        private Dictionary<uint, PvEEntity> _entities;
        private Dictionary<uint, uint[]> _triggerMap;
        private RoomSession _session;

        public PvEEntityManager()
        {
            this._entities = new Dictionary<uint, PvEEntity>();
            this._triggerMap = new Dictionary<uint, uint[]>();
        }

        public void Initialize(RoomSession session)
        {
            this._session = session;
        }

        public void Clear()
        {
            this._entities.Clear();
            this._triggerMap.Clear();
        }

        public void SetupStage(byte stage)
        {
            if(stage == 1)
            {
                Clear();

                // TODO get pos
                PvEAreaTrigger mainGoal = new PvEAreaTrigger(0, GetAvailableId(), new Position()); // TODO;
                this._entities.Add(mainGoal.Uid, mainGoal);

                PvEObject smallDoor = new PvEObject((uint)1, GetAvailableId(), new Position()
                {
                    X = 30f,
                    Y = 0,
                    Z = -30f
                }); // TODO pos
                this._entities.Add(smallDoor.Uid, smallDoor);

                PvEObject bigDoor = new PvEObject((uint)2, GetAvailableId(), new Position()
                {
                    X = 33.9f,
                    Y = 0f, // z
                    Z = -34f 
                });
                this._entities.Add(bigDoor.Uid, bigDoor);

                PvEObject button = new PvEObject((uint)5, GetAvailableId(), new Position()
                {
                    X = 31f,
                    Y = -0.1f,
                    Z = -29f
                });
                this._entities.Add(button.Uid, button);

                PvENpc boss = new PvENpc((uint)13, (uint)100, GetAvailableId(), new Position()
                {
                    X = 20f,
                    Y = 0f,
                    Z = -30f
                });
                this._entities.Add(boss.Uid, boss);

                PvEItem coin1 = new PvEItem((uint)1191182354, GetAvailableId(), new Position()
                {
                    X = 35f,
                    Y = 0f, // z
                    Z = -36f
                });
                this._entities.Add(coin1.Uid, coin1);

                PvEItem coin2 = new PvEItem((uint)1191182354, GetAvailableId(), new Position()
                {
                    X = 33f,
                    Y = 0f, // z
                    Z = -36f
                });
                this._entities.Add(coin2.Uid, coin2);

                // TODO red platforms

                // make links
                this._triggerMap.Add(button.Uid, new uint[] { smallDoor.Uid, bigDoor.Uid });
            }
            else if(stage == 2)
            {
                Clear();

                PvEObject essence = new PvEObject((uint)6, GetAvailableId(), new Position()
                {
                    X = 0f,
                    Y = 0.25f,
                    Z = 0f,
                });
                this._entities.Add(essence.Uid, essence);
            }
            else
            {
                Clear();
            }
        }

        public uint SpawnItem(Items item, Position spawn)
        {
            uint uid = GetAvailableId();
            var newItem = new PvEItem((uint)item, uid, spawn);

            lock (this._entities)
                this._entities.Add(uid, newItem);

            return uid;
        }

        public void SyncPlayer(RoomSessionPlayer player) // send info
        {
            lock(this._entities)
                foreach (var e in this._entities)
                {
                    var type = e.Value.GetType();
                    if (type == typeof(PvEAreaTrigger))
                    {
                        var areatrigger = (PvEAreaTrigger)e.Value;
                        player.Post(new GCPvEAreaTriggerInit(areatrigger.Id, areatrigger.Uid, areatrigger.Position, 0));
                    }else if (type == typeof(PvEItem))
                    {
                        var item = (PvEItem)e.Value;
                        player.Post(new GCPvEItemInit(item.ItemId, item.Uid, item.Position));
                    }
                    else if (type == typeof(PvENpc))
                    {
                        var npc = (PvENpc)e.Value;
                        player.Post(new GCPvENpcInit(npc.Id, npc.Uid, (ushort)0, (byte)0, npc.Position));
                    }else if (type == typeof(PvEObject))
                    {
                        var obj = (PvEObject)e.Value;
                        player.Post(new GCPvEObjectInit(obj.Id, obj.Uid, obj.Position, 0));
                    }
                }
        }

        public void InvokeObject(RoomSessionPlayer player, uint uid, bool trigger = true)
        {
            lock(this._entities)
            {
                foreach(var e in this._entities)
                {
                    if (e.Key != uid)
                        continue;

                    lock(this._triggerMap)
                        if(this._triggerMap.ContainsKey(e.Key))
                        {
                            foreach(var ee in this._triggerMap[e.Key])
                            {
                                if (!this._entities.ContainsKey(ee))
                                    continue;

                                var target = this._entities[ee];
                                InvokeObj(player, target);
                            }
                        }
                    break;
                }
            }
        }

        public void ItemPickup(RoomSessionPlayer player, uint uid)
        {
            lock(this._entities)
                foreach(var e in this._entities)
                {
                    if (e.Key != uid)
                        continue;

                    var item = (PvEItem)e.Value;
                    item.OnTrigger(player);
                    return;
                }
        }

        private void InvokeObj(RoomSessionPlayer player, PvEEntity e)
        {
            if (player == null || player.RoomSession == null)
                return;

            var type = e.GetType();
            if (type == typeof(PvEObject))
            {
                var obj = (PvEObject)e;
                if(obj.Id == 1 || obj.Id == 2) // only care about small one?
                {
                    player.RoomSession.RelayPlaying<GCPvEDoor>(e.Uid, true);
                    return;
                }
            }

            player.RoomSession.RelayPlaying<GCPvEEventObject>(e.Uid, true);
        }

        private uint GetAvailableId()
        {
            uint highest = 10000000; // NOTE: dont conflict with playerid's?
            lock (this._entities)
                foreach (var e in this._entities)
                    if (e.Key > highest)
                        highest = e.Key;

            return highest+1;
        }
    }
}
