using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class KillCondition : AchievementCondition
    {
        private uint _kills;

        public KillCondition(uint killCount)
        {
            this._kills = killCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return player.Player.StatsManager.Kills >= this._kills;
        }
    }
}
