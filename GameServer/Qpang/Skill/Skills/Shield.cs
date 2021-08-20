using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Shackles : Skill
    {
        // Special actions are not available for opponent for15 seconds.

        public override uint GetId()
        {
            return (uint)Items.SKILL_SHACKLES;
        }
    }
}
