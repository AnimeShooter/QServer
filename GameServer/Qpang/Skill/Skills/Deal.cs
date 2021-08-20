using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Deal : Skill
    {
        // Sacrifice all your skill points to reduce opponents skill points by 1 (applies to whole team)  

        public override uint GetId()
        {
            return (uint)Items.SKILL_DEAL;
        }
    }
}
