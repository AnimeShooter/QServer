using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Database.Repositories;
using Qserver.Util;

namespace Qserver.GameServer.Qpang
{
    public class Leaderboard
    {
        public struct PositionAPI
        {
            public uint Rank;
            public string Name;
            public uint Experience;
            public uint Level;
            public uint Kills;
            public uint Deaths;
            public uint MeleeKills;
            public uint GunKills;
            public uint LauncherKills;
            public uint BombKills;
        }
        public struct Position
        {
            public uint Rank;
            public int Difference;
        }

        private Dictionary<uint, Position> _currentRanking;
        private Dictionary<uint, Position> _lastRanking;
        private object _lock;

        public Leaderboard()
        {
            this._currentRanking = new Dictionary<uint, Position>();
            this._lastRanking = new Dictionary<uint, Position>();
            this._lock = new object();
        }

        public List<PositionAPI> List()
        {
            List<PositionAPI> positions = new List<PositionAPI>();
            lock(this._lock)
            {
                foreach (var pos in this._currentRanking)
                {
                    var newPos = new PositionAPI()
                    {
                        Rank = pos.Value.Rank
                    };
                    

                    var player = Game.Instance.GetPlayer(pos.Key);
                    if (player == null)
                    {
                        positions.Add(newPos);
                        continue;
                    }

                    newPos.Name = player.Name;
                    newPos.Experience = player.Experience;
                    newPos.Level = player.Level;
                    newPos.Kills = player.StatsManager.Kills;
                    newPos.Deaths = player.StatsManager.Deaths;
                    newPos.MeleeKills = player.StatsManager.MeleeKills;
                    newPos.GunKills = player.StatsManager.GunKills;
                    newPos.LauncherKills = player.StatsManager.LauncherKills;
                    newPos.BombKills = player.StatsManager.BombKills;

                    positions.Add(newPos);

                    if (positions.Count >= 100)
                        break;
                }    
            }
            return positions;
        }

        public void Refresh()
        {
            lock(this._lock)
            {
                if (this._currentRanking.Count == 0)
                    this._lastRanking = this._currentRanking;

                var players = Game.Instance.PlayersRepository.GetLeaderboardPlayers().Result;

                uint rank = 1;

                foreach(var p in players)
                {
                    var playerId = p.id;

                    Position lastPos;
                    if (this._lastRanking.ContainsKey(playerId))
                        lastPos = this._lastRanking[playerId];
                    else
                        lastPos = new Position() { Rank = rank };

                    Position pos = new Position()
                    {
                        Rank = rank,
                        Difference = (int)lastPos.Rank - (int)rank
                    };

                    if (this._currentRanking.ContainsKey(playerId))
                        this._currentRanking[playerId] = pos;
                    else
                        this._currentRanking.Add(playerId, pos);

                    rank++;
                }

                Log.Message(LogType.MISC, $"Leaderboard loaded {players.Count} players the database!");
            }
        }

        public Position GetPosition(uint playerId)
        {
            lock(this._lock)
            {
                if (this._currentRanking.ContainsKey(playerId))
                    return this._currentRanking[playerId];
            }
            return new Position()
            {
                Difference = 0,
                Rank = 999
            };
        }
    }
}
