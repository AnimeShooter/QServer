using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer
{
    public struct Coupon
    {
        public string Key;
        public uint DonReward;
        public uint CashReward;
        public uint ItemReward;
        public ushort ItemPeriod;
        public byte ItemType;

        //public Coupon(string key, uint donRward, uint cashReward, uint itemReward, uint itemPeriod)
        //{
        //    this._key = key;
        //    this._donReward = donRward;
        //    this._cashReward = cashReward;
        //    this._itemReward = itemReward;
        //    this._itemPeriod = itemPeriod;
        //}

        //public bool Consume()
        //{
        //    // do its thing?

        //    return true;
        //}
    }
}
