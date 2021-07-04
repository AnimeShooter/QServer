﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Database.Repositories
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
				items = connection.QueryAsync<CraneItem>("SELECT name, level, experience, don_reward, cash_reward, coin_reward FROM levels WHERE"));
			return items.Result.ToList();
		}

	}
}
