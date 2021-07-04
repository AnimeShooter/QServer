using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Database.Repositories
{
    public class LevelRepository
    {
		public LevelRepository(IMySqlObjectFactory sqlObjectFactory)
		{
			_sqlObjectFactory = sqlObjectFactory;
		}

		private readonly IMySqlObjectFactory _sqlObjectFactory;

		public async Task<List<Level>> GetLevelInfo()
		{
			Task<IEnumerable<Level>> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection => 
				items = connection.QueryAsync<Level>("SELECT id, item_id, use_up, period, active, created_at, updated_at FROM crane_items WHERE active = 1"));
			return items.Result.ToList();
		}

	}
}
