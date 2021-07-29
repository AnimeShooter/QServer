using Dapper;
using Qserver.GameServer.Qpang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qserver.Database.Repositories
{
    public class PlayersRepository
    {
        public struct DBPlayer
        {
            public uint id;
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
            public string discordId;
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

        public PlayersRepository(IMySqlObjectFactory sqlObjectFactory)
        {
            _sqlObjectFactory = sqlObjectFactory;
        }

        private readonly IMySqlObjectFactory _sqlObjectFactory;

        public async Task<DBPlayer> GetPlayer(uint id)
        {
            Task<IEnumerable<DBPlayer>> items = null;
            await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
                items = connection.QueryAsync<DBPlayer>("SELECT id, user_id, name, default_character, rank, prestige, level, don, cash, coins, experience, is_muted FROM players WHERE id = @Id", new { Id = id }));
            return items.Result.FirstOrDefault();
        }

        public async Task<DBPlayer> GetPlayerByUserId(uint id)
        {
            Task<IEnumerable<DBPlayer>> items = null;
            await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
                items = connection.QueryAsync<DBPlayer>("SELECT id, user_id, name, default_character, rank, prestige, level, don, cash, coins, experience, is_muted, discordId FROM players WHERE user_id = @Id", new { Id = id }));
            return items.Result.FirstOrDefault();
        }

        public async Task UpdatePlayer(Player player)
        {
            await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
                connection.QueryAsync("UPDATE players SET default_character = @DefaultCharacter, don = @Don, cash = @Cash, coins = @Coins, level = @Level, prestige = @Prestige, experience = @Experience WHERE id = @Id",
                new { DefaultCharacter = player.Character, Don = player.Don, Cash = player.Cash, Coins = player.Coins, Level = player.Level, Prestige = player.Prestige, Experience = player.Experience, Id = player.PlayerId }));
        }

        // TODO move to users
        //public async Task UpdatePlayerDiscordId(string discordId)
        //{
        //    await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
        //        connection.QueryAsync("UPDATE players SET discordId = @DiscordId WHERE id = @Id", new { DiscordId = discordId }));
        //}

        public async Task<DBPlayerStats> GetPlayerStats(uint playerId)
        {
            Task<DBPlayerStats> stats = null;
            await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
                stats = connection.QuerySingleAsync<DBPlayerStats>("SELECT kills, deaths, n_won, n_lost, n_drew, m_won, m_drew, playtime, slacker_points, melee_kills, gun_kills, launcher_kills, bomb_kills, headshot_kills, headshot_deaths, team_kills, team_deaths, event_item_pickups FROM player_stats WHERE player_id = @Id", new { Id = playerId }));
            return stats.Result;
        }

        public async Task UpdatePlayerStats(Player player)
        {
            await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
                connection.QuerySingleAsync("UPDATE player_stats SET kills = @Kills, deaths = @Deaths, n_won = @Nwon, n_lost = @Nlost, n_drew = @Ndrew, m_won = @Mwon, m_drew = @MDrew, playtime = @Playtime, slacker_points = @SlackerPoints, melee_kills = @MeleeKills, " +
                "gun_kills = @GunKills, launcher_kills = @LauncherKills, bomb_kills = @BombKills, headshot_kills = @HeadshotKills, headshot_deaths = @HeadshotDeaths, team_kills = @TeamKills, team_deaths = @TeamDeaths, event_item_pickups = @EventItemPickups WHERE id = @Id",
                new
                {
                    Kills = player.StatsManager.Kills,
                    Deaths = player.StatsManager.Deaths,
                    Nwon = player.StatsManager.NormalWins,
                    NLost = player.StatsManager.NormalLosses,
                    NDrew = player.StatsManager.NormalDrews,
                    Playtime = player.StatsManager.PlayTime,
                    SlackerPoints = player.StatsManager.SlackerPoints,
                    MeleeKills = player.StatsManager.MeleeKills,
                    GunKills = player.StatsManager.GunKills,
                    LancherKills = player.StatsManager.LauncherKills,
                    BombKills = player.StatsManager.BombKills,
                    HeadshotKills = player.StatsManager.HeadshotKills,
                    HeadshotDeaths = player.StatsManager.HeadshotDeaths,
                    TeamKills = player.StatsManager.TeamKills,
                    TeamDeaths = player.StatsManager.TeamDeaths,
                    EventItemPickups = 0,
                    Id = player.PlayerId
                }));
        }

        public async Task<uint> GetPlayerId(uint userId)
        {
            Task<uint> res = null;
            await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
                res = connection.QuerySingleAsync<uint>("SELECT id FROM players WHERE user_id = @UserId", new { UserId = userId }));
            return res.Result;
        }

        public async Task<uint> CreatePlayer(uint userid, string name, int don, int cash)
        {
            Task<uint> playerid = null;
            await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
                playerid = connection.QuerySingleAsync<uint>("INSERT INTO players (user_id, name, default_character, rank, prestige, level, don, cash, coins, experience, is_muted) VALUES (@Userid, @Name, @DefaultCharacter, @Rank, @Prestige, @Level, @Don, @Cash, @Coins, @Experience, @IsMuted);  SELECT LAST_INSERT_ID()",
                new { Userid = userid, Name = name, DefaultCharacter = 333, Rank = 1, Prestige = 1, Level = 1, Don = don, Cash = cash, Coins = 0, Experience = 0, IsMuted = 0 }));
            return playerid.Result;
        }

        public async Task CreatePlayerStats(uint playerid)
        {
            await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
                connection.QueryAsync("INSERT INTO player_stats (player_id) VALUES (@Playerid);",
                new { Playerid = playerid }));
        }

        public async Task CreatePlayerEquipments(uint playerid, ushort characterid)
        {
            await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
                connection.QueryAsync("INSERT INTO player_equipment (player_id, character_id, melee, `primary`, secondary, throw, head, face, body, hands, legs, shoes, back, side) VALUES (@Playerid, @Characterid, 0, 0, 0, 0, 0 ,0, 0, 0, 0, 0, 0, 0);",
                new { Playerid = playerid, Characterid = characterid }));
        }

        public async Task<List<DBPlayer>> GetLeaderboardPlayers()
        {
            Task<IEnumerable<DBPlayer>> players = null;
            await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
                players = connection.QueryAsync<DBPlayer>("SELECT id, experience FROM players ORDER BY experience DESC LIMIT 500")); // NOTE: limit 500 to prevent DoS?
            return players.Result.ToList();
        }

        // Load
        // ban
        // mute
        // unmute
        // update

    }
}
