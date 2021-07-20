using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Database.Repositories
{
	public struct DBUser
	{
		public uint id;
		public string name;
		public string email;
		public string password;
		public string session_uuid;
		public string ip;
		public byte whitelisted;
	}

	public class UsersRepository
    {
		


		public UsersRepository(IMySqlObjectFactory sqlObjectFactory)
		{
			_sqlObjectFactory = sqlObjectFactory;
		}

		private readonly IMySqlObjectFactory _sqlObjectFactory;


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


		public async Task UpdateUUID(string username, string uuid)
		{
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				connection.QuerySingleAsync("UPDATE users SET session_uuid = @Uuid WHERE name = @Username", new { Username = username, Uuid = uuid }));
		}

		public async Task<string> GetUserPassword(string username)
		{
			Task<IEnumerable<string>> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				items = connection.QueryAsync<string>("SELECT password FROM users WHERE name = @Username", new { Username = username }));
			return items.Result.FirstOrDefault();
		}

		public async Task<uint> CreateUser(string name, string email, string password, string ip)
        {
			Task<uint> userid = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				userid = connection.QuerySingleAsync<uint>("INSERT INTO users (name, email, password, session_uuid, registration_ip, whitelisted) VALUES (@Name, @Email, @Password, @Uuid, @Ip, @Whitelisted);  SELECT LAST_INSERT_ID()", 
				new { Name = name, Email = email, Password = password, Uuid = Util.Util.GenerateUUID(), Ip = ip, Whitelisted = 1 }));
			return userid.Result;
		}

		public async Task<List<DBUser>> UserExists(string name, string email)
        {
			Task<IEnumerable<DBUser>> users = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				users = connection.QueryAsync<DBUser>("SELECT id, name, email FROM users WHERE name = @Name OR email = @Email",
				new { Name = name, Email = email }));
			return users.Result.ToList();
		}
		


	}
}
