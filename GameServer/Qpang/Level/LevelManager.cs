using Qserver.Database;
using Qserver.Database.Repositories;
using Qserver.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class LevelManager
    {
        private Dictionary<byte, Level> _levels;
        private LevelsRepository _levelRepository;
        public LevelManager()
        {
            this._levelRepository = new LevelsRepository(DatabaseManager.MySqlFactory);
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
            if(level > 0 && level <= 45);
                return this._levels[level];

            return new Level();
        }

        public void OnPlayerFinish(RoomSessionPlayer session)
        {
            var player = session.Player;

            var currentLevel = player.Level;
            var targetLevel = GetLevelForExperience(player.Experience, 0);

            if (targetLevel.Lvl <= currentLevel)
                return;

            for(int i = currentLevel+1; i <= targetLevel.Lvl; i++)
            {
                var rewards = GetLevelRewards((byte)i);
                player.AddDon(rewards.DonReward);
            }

            player.Level = targetLevel.Lvl;
        }
    }
}
