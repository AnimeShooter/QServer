using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public abstract class AchievementCondition
    {
        public virtual bool IsMatch(RoomSessionPlayer player)
        {
            return false;
        }

    }
}
