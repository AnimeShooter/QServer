using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qserver.GameServer.Qpang;

namespace Qserver.Database.Repositories
{
    public class CraneRepository
    {
		public CraneRepository(IMySqlObjectFactory sqlObjectFactory)
		{
			_sqlObjectFactory = sqlObjectFactory;
		}

		private readonly IMySqlObjectFactory _sqlObjectFactory;

		public async Task<List<CraneItem>> GetCraneItems()
		{
			Task<IEnumerable<CraneItem>> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection => 
				items = connection.QueryAsync<CraneItem>("SELECT id, item_id, use_up, period, active, created_at, updated_at FROM crane_items WHERE active = 1"));
			return items.Result.ToList();
		}

	}
}
