using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class LauncherKillCondition : AchievementCondition
    {
        private uint _kills;

        public LauncherKillCondition(uint killCount)
        {
            this._kills = killCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return player.Player.StatsManager.LauncherKills >= this._kills;
        }
    }
}
