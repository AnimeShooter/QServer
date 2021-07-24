using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Database.Repositories
{
    public class MemoRepository
    {
		public struct DBMemo
        {
			// Memos
			public uint id;
			public uint sender_id;
			public uint receiver_id;
			public string message;
			public byte opened;
			public uint created;

			// Players
			public string Name;
        }

		public MemoRepository(IMySqlObjectFactory sqlObjectFactory)
		{
			_sqlObjectFactory = sqlObjectFactory;
		}

		private readonly IMySqlObjectFactory _sqlObjectFactory;

		public async Task<List<DBMemo>> GetMemos(uint playerId)
		{
			Task<IEnumerable<DBMemo>> memos = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				memos = connection.QueryAsync<DBMemo>("SELECT memos.id, memos.sender_id, memos.receiver_id, memos.message, memos.opened, memos.created, players.name FROM memos JOIN players ON players.id = memos.sender_id WHERE receiver_id = @Id", new { Id = playerId }));
			return memos.Result.ToList();
		}

	}
}
