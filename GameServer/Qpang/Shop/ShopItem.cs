using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public struct ShopItem
    {
        public uint SeqId;
        public uint ItemId;
        public bool IsCash;
        public uint Price;
        public uint SoldCount;
        public uint Stock;
        public byte ShopCategory;
        public byte Type;
        public byte PeriodType;
        public ushort Period;
        public ushort MinLevel;
    }
}
