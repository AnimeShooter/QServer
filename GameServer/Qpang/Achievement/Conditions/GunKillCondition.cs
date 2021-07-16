using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class GunKillCondition : AchievementCondition
    {
        private uint _kills;

        public GunKillCondition(uint killCount)
        {
            this._kills = killCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return player.Player.StatsManager.Deaths >= this._kills;
        }
    }
}
