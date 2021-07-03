using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public struct Friend
    {
        public uint PlayerId;
        public string Nickname;
        public byte Level;
        public byte Rank;
        public byte State;
        public bool IsOnline;
    }
}
