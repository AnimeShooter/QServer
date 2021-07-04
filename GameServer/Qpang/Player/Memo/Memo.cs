using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public struct Memo
    {
        public ulong Id;
        public uint SenderId;
        public string Nickname;
        public string Message;
        public bool IsOpened;
        public DateTime Created;
    }
}
