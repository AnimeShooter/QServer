using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Stunt : Skill
    {
        // Reduce 25% of total damage taken.

        public override uint GetId()
        {
            return (uint)Items.SKILL_STUNT;
        }

        public override uint GetDuration()
        {
            return 30;
        }
    }
}
