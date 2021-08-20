using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Revers : Skill
    {
        // Confuse the opponents by changing their keyboard controls for a short period of time. 
        public override uint GetId()
        {
            return (uint)Items.SKILL_REVERS;
        }

        public override uint GetDuration()
        {
            return 15;
        }
    }
}
