using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public struct EventAPI
    {
        public string Name { get; set; }
        public uint StartTime { get; set; }
        public uint EndTime { get; set; }
    }

    public class Event
    {
        public string Name;
        public uint StartTime;
        public uint EndTime;
        public Dictionary<uint, InventoryCard> PricePool;

        public EventAPI ToAPI()
        {
            return new EventAPI()
            {
                Name = Name,
                StartTime = StartTime,
                EndTime = EndTime
            };
        }
    }

}
