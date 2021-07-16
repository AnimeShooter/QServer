using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class TeamWinCondition : AchievementCondition
    {
        private uint _wins;

        public TeamWinCondition(uint winCount)
        {
            this._wins = winCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return player.Player.StatsManager.NormalWins >= this._wins;
        }
    }
}
