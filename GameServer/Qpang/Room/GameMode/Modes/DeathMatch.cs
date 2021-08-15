using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class DeathMatch : GameMode
    {
        public override bool IsMissionMode()
        {
            return false;
        }

        public override bool IsTeamMode()
        {
            return false;
        }
    }
}
