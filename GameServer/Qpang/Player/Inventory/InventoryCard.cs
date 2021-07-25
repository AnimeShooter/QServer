using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public struct InventoryCard
    {
        public ulong Id;
        public uint PlayerOwnedId;
        public uint ItemId;
        public byte Type;
        public byte PeriodeType;
        public ushort Period;
        public bool IsActive;
        public bool IsOpened;
        public bool IsGiftable;
        public byte BoostLevel;
        public uint TimeCreated;

        public static InventoryCard FromShopItem(ShopItem item)
        {
            return new InventoryCard()
            {
                Id = 0,
                PlayerOwnedId = 0,
                ItemId = item.ItemId,
                Type = item.Type,
                PeriodeType = item.PeriodType,
                Period = item.Period,
                IsActive = false,
                IsOpened = true,
                IsGiftable = true,
                BoostLevel = 0,
                TimeCreated = Util.Util.Timestamp()
            };
        }
    }
}
