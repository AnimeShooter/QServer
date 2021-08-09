using Qserver.GameServer.Qpang;
using Qserver.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer
{
    public class CouponManager
    {
        private object _lock;

        private Dictionary<string, Coupon> _coupons;

        public CouponManager()
        {
            this._lock = new object();
            this._coupons = new Dictionary<string, Coupon>();

            lock(this._lock)
            {
                var coupons = Game.Instance.CouponsRepository.GetActiveCoupons().Result;
                foreach(var c in coupons)
                {
                    this._coupons.Add(c.code, new Coupon()
                    {
                        Key = c.code,
                        CashReward = c.reward_cash,
                        DonReward = c.reward_don,
                        ItemReward = c.reward_itemId,
                        ItemPeriod = c.reward_item_period,
                        ItemType = c.reward_item_type
                    });
                }
                Log.Message(LogType.MISC, $"CouponManager loaded {this._coupons.Count} Coupons from the database!");
            }
        }

        public bool ConsumeCoupon(Player player, string code)
        {
            if (player.TestRealm)
                return false; // no rewards will be lost!

            lock (this._lock)
            {
                if (!this._coupons.ContainsKey(code))
                    return false;

                var coupon = this._coupons[code];

                if (coupon.CashReward != 0)
                    player.AddCash(coupon.CashReward);

                if (coupon.DonReward != 0)
                    player.AddCash(coupon.DonReward);

                if(coupon.ItemReward != 0)
                {
                    var cardReward = new InventoryCard()
                    {
                        IsActive = false,
                        IsGiftable = true,
                        IsOpened = false,
                        ItemId = (uint)coupon.ItemReward,
                        Period = (ushort)coupon.ItemPeriod,
                        PeriodeType = 3,
                        BoostLevel = 0,
                        Type = coupon.ItemType,
                        PlayerOwnedId =  player.PlayerId,
                        TimeCreated = Util.Util.Timestamp()
                    };
                    cardReward.Id = Game.Instance.ItemsRepository.CreateItem(cardReward, player).Result;
                    player.InventoryManager.ReceiveGift(cardReward, "AnimeShooter.com");
                }

                // Use and remove to prevent doops ;P
                Game.Instance.CouponsRepository.UpdateCoupon(code, player.PlayerId).GetAwaiter().GetResult();
                this._coupons.Remove(code);
            }
            return true;
        }

        // NOTE: generate coupon(s)?
    }
}
