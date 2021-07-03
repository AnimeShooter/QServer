using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public struct Channel
    {
        public uint Id;
        public string Name;
        public byte MinLevel;
        public byte MaxLevel;
        public ushort MaxPlayers;
        public ushort CurrPlayers;
    }

    public class ChannelManager
    {
    }
}
