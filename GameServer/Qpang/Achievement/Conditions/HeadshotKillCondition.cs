using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class HeadshotKillCondition : AchievementCondition
    {
        private uint _kills;

        public HeadshotKillCondition(uint killCount)
        {
            this._kills = killCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return player.Player.StatsManager.HeadshotKills >= this._kills;
        }
    }
}
