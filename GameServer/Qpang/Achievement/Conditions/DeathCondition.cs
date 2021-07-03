using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class DeathCondition : AchievementCondition
    {
        private uint _deaths;

        public DeathCondition(uint deathCount)
        {
            this._deaths = deathCount;
        }

        //public override bool IsMatch(RoomSessionPlayer player)
        //{
        //    return player.Player.StatsManager.Deaths >= this._deaths;
        //}
    }
}
