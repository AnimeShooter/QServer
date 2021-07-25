using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Database.Repositories
{
    public class ChannelsRepository
    {
		public ChannelsRepository(IMySqlObjectFactory sqlObjectFactory)
		{
			_sqlObjectFactory = sqlObjectFactory;
		}

		private readonly IMySqlObjectFactory _sqlObjectFactory;

		public async Task<List<Channel>> GetChannels()
		{
			Task<IEnumerable<Channel>> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				items = connection.QueryAsync<Channel>("SELECT id, Name, MinLevel, MaxLevel, MaxPlayers, MinRank, IP, TestMode FROM channels"));
			return items.Result.ToList();
		}

	}
}
