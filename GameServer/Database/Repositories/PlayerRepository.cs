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
		public struct DBPlayer
        {
			public ulong id;
			public uint user_id;
			public string name;
			public ushort default_character;
			public byte rank;
			public byte prestige;
			public byte level;
			public uint don;
			public uint cash;
			public ushort coins;
			public uint experience;
			public byte is_muted;
			public DateTime created_at;
			public DateTime updated_at;
        }

		public PlayerRepository(IMySqlObjectFactory sqlObjectFactory)
		{
			_sqlObjectFactory = sqlObjectFactory;
		}

		private readonly IMySqlObjectFactory _sqlObjectFactory;

		public async Task<DBPlayer> GetPlayer(uint id)
		{
			Task<IEnumerable<DBPlayer>> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection => 
				items = connection.QueryAsync<DBPlayer>("SELECT id, user_id, name, default_character, rank, prestige, level, don, cash, coins, experience, is_muted, created_at, updated_at FROM players WHERE id = @Id", new {Id = id} ));
			return items.Result.FirstOrDefault();
		}

		public async Task<uint> GetUserId(string uuid)
		{
			Task<IEnumerable<uint>> res = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				res = connection.QueryAsync<uint>("SELECT id FROM users WHERE session_uuid = @Uuid", new { Uuid = uuid }));
			return res.Result.FirstOrDefault(); // may be 0
		}

		public async Task<uint> GetPlayerId(uint userId)
		{
			Task<uint> res = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				res = connection.QuerySingleAsync<uint>("SELECT id FROM players WHERE user_id = @UserId", new { UserId = userId }));
			return res.Result;
		}

		public async Task<string> GetUserPassword(string username)
		{
			Task<IEnumerable<string>> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				items = connection.QueryAsync<string>("SELECT password FROM users WHERE name = @Username", new { Username = username }));
			return items.Result.FirstOrDefault();
		}

		public async Task UpdateUUID(string username, string uuid)
		{
			Task<int> res;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				connection.QuerySingleAsync("UPDATE users SET session_uuid = @Uuid WHERE name = @Username", new { Username = username, Uuid = uuid }));
		}

		// Load
		// ban
		// mute
		// unmute
		// update

	}
}
