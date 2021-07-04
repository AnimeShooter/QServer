using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Database.Repositories
{
    public class PlayerRepository
    {
		public PlayerRepository(IMySqlObjectFactory sqlObjectFactory)
		{
			_sqlObjectFactory = sqlObjectFactory;
		}

		private readonly IMySqlObjectFactory _sqlObjectFactory;

		public async Task<Player> GetPlayer(int id)
		{
			throw new Exception("Not implemented database");
			Task<IEnumerable<Player>> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection => 
				items = connection.QueryAsync<Player>("SELECT ... FROM players WHERE id = @Id", new {Id = id} ));
			return items.Result.FirstOrDefault();
		}

	}
}
