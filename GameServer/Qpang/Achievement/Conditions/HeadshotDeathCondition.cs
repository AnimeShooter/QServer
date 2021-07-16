using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class HeadshotDeathCondition : AchievementCondition
    {
        private uint _deaths;

        public HeadshotDeathCondition(uint deahtCount)
        {
            this._deaths = deahtCount;
        }

        public override bool IsMatch(RoomSessionPlayer player)
        {
            return player.Player.StatsManager.HeadshotDeaths >= this._deaths;
        }
    }
}
