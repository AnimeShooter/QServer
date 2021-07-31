using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class RoomManager
    {
        private object _lock = new object();
        private Dictionary<uint, Room> _rooms;
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

        public void Tick()
        {
            lock (this._lock)
            {
                foreach (var room in _rooms)
                    room.Value.Tick();
            }
        }

        public List<Room> List()
        {
            lock (this._lock)
            {
                List<Room> rooms = new List<Room>();
                foreach (var room in _rooms)
                    rooms.Add(room.Value);
                return rooms;
            }
        }

        public Room Create(string name, byte map, GameMode.Mode mode, uint host = 0)
        {
            var id = GetAvailableRoomId();
            Room room;
            if (host != 0)
                room = new Room(id, name, map, mode, host, (ushort)Settings.SERVER_PORT_ROOM);
            else
                room = new Room(id, name, map, mode, Settings.SERVER_IP, (ushort)Settings.SERVER_PORT_ROOM);
            lock(this._lock)
                _rooms.Add(id, room);

            return room;
                
        }

        public void Remove(uint id)
        {
            lock(this._lock)
            {
                if (_rooms.ContainsKey(id))
                    _rooms.Remove(id);
            }
        }

        public Room Get(uint id)
        {
            lock(this._lock)
            {
                if (_rooms.ContainsKey(id))
                    return _rooms[id];

                return null;
            }
        }

       
        private uint GetAvailableRoomId()
        {
            uint id = 1;
            lock (this._lock)
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
