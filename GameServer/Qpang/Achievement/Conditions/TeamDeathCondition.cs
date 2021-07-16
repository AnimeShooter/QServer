using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class TeamDeathCondition : AchievementCondition
    {
        private uint _deaths;

        public TeamDeathCondition(uint deathCount)
        {
            this._deaths = deathCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return player.Player.StatsManager.TeamDeaths >= this._deaths;
        }
    }
}
