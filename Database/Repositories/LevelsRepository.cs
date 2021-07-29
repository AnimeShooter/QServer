using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qserver.GameServer.Qpang;

namespace Qserver.Database.Repositories
{
    public class LevelsRepository
    {
		public LevelsRepository(IMySqlObjectFactory sqlObjectFactory)
		{
			_sqlObjectFactory = sqlObjectFactory;
		}

		private readonly IMySqlObjectFactory _sqlObjectFactory;

		public async Task<List<Level>> GetLevelInfo()
		{
			Task<IEnumerable<Level>> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection => 
				items = connection.QueryAsync<Level>("SELECT Name, Lvl, MinExperience, DonReward, CashReward, CoinReward FROM levels"));
			return items.Result.ToList();
		}

	}
}
