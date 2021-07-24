using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Database.Repositories
{
    public class FriendsRepository
    {
		public struct DBFriend
        {
			// Friends
			public uint id;
			public uint player_from;
			public uint player_to;
			public byte status;

			// Players
			public string Name;
			public byte Level;
			public byte Rank;
        }

		public FriendsRepository(IMySqlObjectFactory sqlObjectFactory)
		{
			_sqlObjectFactory = sqlObjectFactory;
		}

		private readonly IMySqlObjectFactory _sqlObjectFactory;

		public async Task<List<DBFriend>> GetFriends(uint playerId)
		{
			Task<IEnumerable<DBFriend>> friends = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				friends = connection.QueryAsync<DBFriend>("SELECT friends.id, friends.player_from, friends.player_to, friends.status, players.name, players.level, players.rank FROM friends JOIN player on player.id = friends.player_to WHERE friends.player_from = @Id", new { Id = playerId }));
			return friends.Result.ToList();
		}

		public async Task AddFriend(uint playerFrom, uint playerTo, byte status)
		{
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				connection.QueryAsync("INSERT INTO friends (player_from, player_to, status) VALUES ()", new { FromId = playerFrom, ToId = playerTo, Status = status }));
		}

		public async Task UpdateFriendState(uint playerId, uint friendId, byte state)
		{
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				connection.QueryAsync("UPDATE friends SET status = @State WHERE player_from = @PlayerId AND player_to = @FriendId", new {PlayerId = playerId, FriendId = friendId, State = state  }));
		}

		public async Task RemoveFriend(uint playerId, uint friendId)
		{
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				connection.QueryAsync("DELETE FROM friends WHERE player_from = @PlayerId AND player_to = @FriendId", new { PlayerId = playerId, FriendId = friendId }));
		}

	}
}
