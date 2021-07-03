using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Room.GameMode.Modes
{
    public class TeamDeathMatch : GameMode
    {
        public override bool IsTeamMode()
        {
            return true;
        }
    }
}
