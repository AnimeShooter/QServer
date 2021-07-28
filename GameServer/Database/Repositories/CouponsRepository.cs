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

		// AddCoupon

		// UpdateCoupon
	}
}
