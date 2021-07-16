using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class NormalWinCondition : AchievementCondition
    {
        private uint _wins;

        public NormalWinCondition(uint winCount)
        {
            this._wins = winCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return player.Player.StatsManager.NormalWins >= this._wins;
        }
    }
}
