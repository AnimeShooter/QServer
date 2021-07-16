using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class MultiKillCondition : AchievementCondition
    {
        private uint _kills;

        public MultiKillCondition(uint killCount)
        {
            this._kills = killCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return false;
            // TODO player.HighestMultiKill
            // return player.Player.H >= this._kills;
        }
    }
}
