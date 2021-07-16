using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class MissionWinCondition : AchievementCondition
    {
        private uint _wins;

        public MissionWinCondition(uint winCount)
        {
            this._wins = winCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return player.Player.StatsManager.MissionWins >= this._wins;
        }
    }
}
