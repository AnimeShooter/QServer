using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class TeamLoseCondition : AchievementCondition
    {
        private uint _losses;

        public TeamLoseCondition(uint lossCount)
        {
            this._losses = lossCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return player.Player.StatsManager.NormalLosses >= this._losses;
        }
    }
}
