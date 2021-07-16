using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class MissionLoseCondition : AchievementCondition
    {
        private uint _losses;

        public MissionLoseCondition(uint lossCount)
        {
            this._losses = lossCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return player.Player.StatsManager.MissionLosses >= this._losses;
        }
    }
}
