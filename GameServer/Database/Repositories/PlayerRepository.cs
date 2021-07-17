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

		public struct DBPlayerStats
        {
			public uint kills;
			public uint deaths;
			public uint n_won;
			public uint n_lost;
			public uint n_drew;
			public uint m_won;
			public uint m_lost;
			public uint m_drew;
			public uint playtime;
			public uint slacker_points;
			public uint melee_kills;
			public uint gun_kills;
			public uint launcher_kills;
			public uint bomb_kills;
			public uint headshot_kills;
			public uint headshot_deaths;
			public uint team_kills;
			public uint team_death;
			public uint event_item_pickups;

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

		public async Task UpdatePlayer(Player player)
		{
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				connection.QuerySingleAsync("UPDATE players SET default_character = @DefaultCharacter, don = @Don, cash = @Cash, coins = @Coins, level = @Level, prestiege = @Prestiege, experience = @Experience WHERE id = @Id",
				new { DefaultCharacter = player.Character, Don = player.Don, Cash = player.Cash, Coins = player.Coins, Level = player.Level, Prestiege = player.Prestige, Experience = player.Experience, Id = player.PlayerId }));
		}

		public async Task<DBPlayerStats> GetPlayerStats(uint playerId)
		{
			Task<DBPlayerStats> stats = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				stats = connection.QuerySingleAsync<DBPlayerStats>("SELECT kills, deaths, n_won, n_lost, n_drew, m_won, m_drew, playtime, slacker_points, melee_kills, gun_kills, launcher_kills, bomb_kills, headshot_kills, headshot_deaths, team_kills, team_deaths, event_item_pickups FROM player_stats WHERE player_id = @Id", new {Id = playerId }));
			return stats.Result;
		}

		public async Task UpdatePlayerStats(Player player)
		{
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				connection.QuerySingleAsync("UPDATE player_stats SET kills = @Kills, deaths = @Deaths, n_won = @Nwon, n_lost = @Nlost, n_drew = @Ndrew, m_won = @Mwon, m_drew = @MDrew, playtime = @Playtime, slacker_points = @SlackerPoints, melee_kills = @MeleeKills, " +
				"gun_kills = @GunKills, launcher_kills = @LauncherKills, bomb_kills = @BombKills, headshot_kills = @HeadshotKills, headshot_deaths = @HeadshotDeaths, team_kills = @TeamKills, team_deaths = @TeamDeaths, event_item_pickups = @EventItemPickups WHERE id = @Id",
				new { Kills = player.StatsManager.Kills,
					Deaths = player.StatsManager.Deaths,
					Nwon = player.StatsManager.NormalWins, NLost = player.StatsManager.NormalLosses, NDrew = player.StatsManager.NormalDrews, 
					  Playtime = player.StatsManager.PlayTime, SlackerPoints = player.StatsManager.SlackerPoints, MeleeKills = player.StatsManager.MeleeKills, GunKills = player.StatsManager.GunKills,
					  LancherKills = player.StatsManager.LauncherKills, BombKills = player.StatsManager.BombKills, HeadshotKills = player.StatsManager.HeadshotKills, HeadshotDeaths = player.StatsManager.HeadshotDeaths,
					  TeamKills = player.StatsManager.TeamKills, TeamDeaths = player.StatsManager.TeamDeaths, EventItemPickups = 0, Id = player.PlayerId}));
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
