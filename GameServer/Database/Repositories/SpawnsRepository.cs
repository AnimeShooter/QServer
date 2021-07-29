using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Database.Repositories
{
    public class SpawnsRepository
    {
		public struct DBSpawns
        {
			// map_spawns
			// public uint id
			public byte map_id;
			public byte game_mode_id;
			public uint position_id;
			public byte team;

			//  position
			// public uint id
			public float x;
			public float y;
			public float z;
			//public string name;

			// map
			// public uint id
			//public uint map_id;
			//public string name; 

			// game mode
			// public uint id
			public byte mode_id;
			// public string name
			
        }

		public struct DBGameItemSpawns
        {
			// game_item_spawn
			public uint id;

			// maps
			public byte map_id;

			// position
			public float x;
			public float y;
			public float z;
        }

		public SpawnsRepository(IMySqlObjectFactory sqlObjectFactory)
		{
			_sqlObjectFactory = sqlObjectFactory;
		}

		private readonly IMySqlObjectFactory _sqlObjectFactory;

		public async Task<List<DBSpawns>> GetSpawns()
		{
			Task<IEnumerable<DBSpawns>> spawns = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				spawns = connection.QueryAsync<DBSpawns>("SELECT map_spawns.map_id, map_spawns.game_mode_id, map_spawns.position_id, map_spawns.team, positions.x, positions.y, positions.x, game_modes.mode_id FROM map_spawns INNER JOIN positions ON positions.id = map_spawns.position_id INNER JOIN maps ON maps.id = map_spawns.map_id INNER JOIN game_modes ON game_modes.id = map_spawns.game_mode_id"));
			return spawns.Result.ToList();
		}

		public async Task<List<DBGameItemSpawns>> GetGameItemSpawns()
		{
			Task<IEnumerable<DBGameItemSpawns>> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				items = connection.QueryAsync<DBGameItemSpawns>("SELECT game_item_spawns.id, maps.map_id, positions.x, positions.y, positions.z FROM game_item_spawns INNER JOIN positions ON positions.id = game_item_spawns.position_id INNER JOIN maps ON maps.id = game_item_spawns.map_id"));
			return items.Result.ToList();
		}

		// TODO
		//public async Task<List<DBMemo>> DeleteMemo(uint id)
		//{
		//	Task<IEnumerable<DBMemo>> memos = null;
		//	await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
		//		memos = connection.QueryAsync<DBMemo>("SELECT memos.id, memos.sender_id, memos.receiver_id, memos.message, memos.opened, memos.created, players.name FROM memos JOIN players ON players.id = memos.sender_id WHERE receiver_id = @Id", new { Id = playerId }));
		//	return memos.Result.ToList();
		//}

	}
}
