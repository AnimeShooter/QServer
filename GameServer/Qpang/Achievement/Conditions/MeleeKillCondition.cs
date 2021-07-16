using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class MeleeKillCondition : AchievementCondition
    {
        private uint _kills;

        public MeleeKillCondition(uint killCount)
        {
            this._kills = killCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return player.Player.StatsManager.MeleeKills >= this._kills;
        }
    }
}
