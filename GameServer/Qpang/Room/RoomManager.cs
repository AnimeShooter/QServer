using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class RoomManager
    {
        private object _lock;

        private Dictionary<uint, Room> _rooms;
        private GameModeManager _gameModeManager;

        public RoomManager()
        {
            this._lock = new object();
            this._rooms = new Dictionary<uint, Room>();

        }
    }
}
