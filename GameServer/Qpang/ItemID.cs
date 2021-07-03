using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public enum Items : uint
    {
		EXP_MAKER_25 = 1174405140,
		DON_MAKER_25 = 1174405141,
		EXP_MAKER_50 = 1174405157,
		DON_MAKER_50 = 1174405182,
		SLACKER_BLESSING = 1174405142,
		QUICK_REVIVE = 1174405146,
		NAME_CHANGER = 1174405145,
		KD_CLEANER = 1174405143,
		WL_CLEANER = 1174405144,
		CRANE_MACHINE_ELITE = 1174405151,
		GOLD_WRENCH = 1174405221,
		FRIEND_MAKER = 1174405150,
		ESSENCE_CHIP = 1174405152,
		PVE_COIN_BOOSTER = 1174405220,
		PVE_PANTH_CHEST = 1174405162,
		PVE_PANTH_KEY = 1174405165,
		PVE_PANTH_BLESSING = 1174405164,

		SKILL_ASSASSIN = 1258356765,
		SKILL_CAMO = 1258356753,
		SKILL_ABSORB = 1258356771,
		SKILL_TEAM_CHEER = 1258356770,
		SKILL_ZILLA = 1258356778,
	}

	public static class ItemID
    {
		public static bool IsEquippableFunction(uint id)
		{
			return id == (uint)Items.EXP_MAKER_25 ||
				id == (uint)Items.EXP_MAKER_50 ||
				id == (uint)Items.DON_MAKER_25 ||
				id == (uint)Items.DON_MAKER_50 ||
				id == (uint)Items.QUICK_REVIVE ||
				id == (uint)Items.PVE_COIN_BOOSTER ||
				id == (uint)Items.PVE_PANTH_BLESSING;
		}
	}
}
