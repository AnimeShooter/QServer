using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class StreakCondition : AchievementCondition
    {
        private uint _kills;

        public StreakCondition(uint killCount)
        {
            this._kills = killCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return false;
            // TODO player.HighestStreak
            //return player.Player.S >= this._kills;
        }
    }
}
