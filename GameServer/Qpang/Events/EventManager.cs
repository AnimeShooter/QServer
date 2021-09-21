using Qserver.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class EventManager
    {
        private object _lock;
        private Dictionary<uint, Event> _events;
        private uint _lastTick;

        public EventManager()
        {
            this._lock = new object();
            this._events = new Dictionary<uint, Event>();
            this._lastTick = 0;

            lock(this._lock)
            {
                Log.Message(LogType.MISC, $"EventManager loaded {this._events.Count} Events from the database!");
            }
        }
        
        public void Tick()
        {
            uint currTime = Util.Util.Timestamp();
            if (_lastTick + 30 > currTime)
                return; // only check every 30 sec


        }
    }
}
