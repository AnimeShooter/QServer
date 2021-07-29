using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Database.Repositories;
using Qserver.Util;

namespace Qserver.GameServer.Qpang
{
    public class Leaderboard
    {
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
