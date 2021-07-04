using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public struct CraneItem
    {
        public int id { get; set; }
        public ulong item_id { get; set; }
        public uint item_type { get; set; }
        public ushort use_up { get; set; }
        public ushort period { get; set; }
        public bool active { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
