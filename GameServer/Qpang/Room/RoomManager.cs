﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class RoomManager
    {
        private static object _lock = new object();
        private static Dictionary<uint, Room> _rooms;
        private GameModeManager _gameModeManager;

        public GameModeManager GameModeManager
        {
            get { return this._gameModeManager; }
        }

        public RoomManager()
        {
            this._gameModeManager = new GameModeManager();
            _rooms = new Dictionary<uint, Room>();
        }

        public static void Tick()
        {
            lock (_lock)
            {
                foreach (var room in _rooms)
                    room.Value.Tick();
            }
        }

        public List<Room> List()
        {
            lock (_lock)
            {
                List<Room> rooms = new List<Room>();
                foreach (var room in _rooms)
                    rooms.Add(room.Value);
                return rooms;
            }
        }

        public Room Create(string name, byte map, byte mode)
        {
            var id = GetAvailableRoomId();
            //var room = new Room(id, name, map, mode, 0xA4447A93, (ushort)Settings.SERVER_PORT_ROOM);
            var room = new Room(id, name, map, mode, 0x7F000001, (ushort)Settings.SERVER_PORT_ROOM);
            lock(_lock)
            {
                _rooms.Add(id, room);
            }

            return room;
                
        }

        public void Remove(uint id)
        {
            lock(_lock)
            {
                if (_rooms.ContainsKey(id))
                    _rooms.Remove(id);
            }
        }

        public Room Get(uint id)
        {
            lock(_lock)
            {
                if (_rooms.ContainsKey(id))
                    return _rooms[id];

                return null;
            }
        }

       
        private uint GetAvailableRoomId()
        {
            uint id = 1;
            lock (_lock)
            {
                
                while(true)
                {
                    if (!_rooms.ContainsKey(id))
                        break;
                    id++;
                }
            }
            return id;
        }
    }
}
