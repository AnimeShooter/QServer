using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Database.Repositories
{
    public class CouponsRepository
    {
		public struct DBCoupon
        {
			public uint id;
			public string code;
			public uint reward_itemId;
			public ushort reward_item_period;
			public byte reward_item_type;
			public uint reward_cash;
			public uint reward_don;
			public uint consumer_id;
        }

		public CouponsRepository(IMySqlObjectFactory sqlObjectFactory)
		{
			_sqlObjectFactory = sqlObjectFactory;
		}

		private readonly IMySqlObjectFactory _sqlObjectFactory;

		public async Task<List<DBCoupon>> GetActiveCoupons()
		{
			Task<IEnumerable<DBCoupon>> coupons = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				coupons = connection.QueryAsync<DBCoupon>("SELECT id, code, reward_cash, reward_don, reward_itemId, reward_item_period, reward_item_type FROM coupons WHERE consumer_id = 0"));
			return coupons.Result.ToList();
		}

		public async Task<uint> AddCoupon(string code, uint cashReward, uint donReward, uint rewardItemId,  ushort rewardItemPeriod, byte rewardItemType )
		{
			Task<uint> coupondid = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				coupondid = connection.QuerySingleAsync<uint>("INSERT INTO coupons (code, reward_cash, reward_don, reward_itemId, reward_item_period, reward_item_type) VALUES (@Code, @CashReward, @DonReward, @RewardItemId, @RewardItemPeriod, @RewardItemType); SELECT LAST_INSERT_ID()",
				new { Code = code, CashReward =cashReward, DonReward = donReward, RewardItemId = rewardItemId, RewardItemPeriod = rewardItemPeriod, RewardItemType = rewardItemType }));
			return coupondid.Result;
		}

		public async Task UpdateCoupon(string code, uint consumerId)
		{
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				connection.QueryAsync("UPDATE coupons SET consumer_id = @ConsumerId WHERE code = @Code", new { Code = code, ConsumerId = consumerId }));
		}
	}
}
