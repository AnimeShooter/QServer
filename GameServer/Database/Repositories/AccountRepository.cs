using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Qserver.GameServer.Database.Repositories
{
    public class AccountRepository
    {
		public AccountRepository(IMySqlObjectFactory sqlObjectFactory)
		{
			_sqlObjectFactory = sqlObjectFactory;
		}

		private readonly IMySqlObjectFactory _sqlObjectFactory;

		public async Task<int> testtest()
		{
			Task<int> count = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				count = connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM pf_ErrorLog"));
			return await count;
		}

	}
}
