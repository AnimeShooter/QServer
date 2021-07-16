using Qserver.GameServer.Database;
using Qserver.GameServer.Database.Repositories;
using Qserver.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class LevelManager
    {
        private Dictionary<byte, Level> _levels;
        private LevelRepository _levelRepository;
        public LevelManager()
        {
            this._levelRepository = new LevelRepository(DatabaseManager.MySqlFactory);
            this._levels = new Dictionary<byte, Level>();
            foreach(var l in this._levelRepository.GetLevelInfo().Result)
            {
                this._levels.Add(l.Lvl, l);
            }
            Log.Message(LogType.MISC, $"LevelManger loaded {this._levels.Count} levels from the database!");
        }

        public Level GetLevelForExperience(uint exp, byte level)
        {
            foreach(var lvl in this._levels)
            {
                if (lvl.Value.MinExperience < exp)
                    continue;
                else
                {
                    if (lvl.Key == 1)
                        return lvl.Value;
                    return GetLevelRewards((byte)(lvl.Key - 1));
                }
            }
            return new Level();
        }

        public Level GetLevelRewards(byte level)
        {
            // assert(level > 0 && level <= 45);

            return this._levels[level];
        }

        public void OnPlayerFinish(RoomSessionPlayer session)
        {
            var player = session.Player;

            //var currentLevel = player.Level

            // TODO: levelup if needed
        }
    }
}
