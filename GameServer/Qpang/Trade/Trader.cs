using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang.Trade
{
    public struct Trader
    {
        public uint PlayerId;
        public uint TargetId;
        public List<InventoryCard> PlayerCards;
        public List<InventoryCard> TargetCards;

    }
}
