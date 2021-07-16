using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class TeamKillCondition : AchievementCondition
    {
        private uint _kills;

        public TeamKillCondition(uint killCount)
        {
            this._kills = killCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return player.Player.StatsManager.TeamKills >= this._kills;
        }
    }
}
