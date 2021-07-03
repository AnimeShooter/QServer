using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Achievement.Conditions
{
    public class BombKillCondition : AchievementCondition
    {
        private uint _kills;

        public BombKillCondition(uint killCount)
        {
            this._kills = killCount;
        }

        //public override bool IsMatch(RoomSessionPlayer player)
        //{
        //    return player.Player.StatsManager.BombKills >= this._kills;
        //}
    }
}
