using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public struct Channel
    {
        public ushort Id;
        public string Name;
        public byte MinLevel;
        public byte MaxLevel;
        public ushort MaxPlayers;
        public ushort MinRank;
        public string IP;
        public ushort CurrPlayers;
        public byte TestMode;
    }
}
