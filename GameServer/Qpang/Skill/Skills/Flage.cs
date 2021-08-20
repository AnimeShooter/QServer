using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Flage : Skill
    {
        // Go undercover by making your team members look like the opposite team. 

        public override uint GetId()
        {
            return (uint)Items.SKILL_FLAGE;
        }
    }
}
