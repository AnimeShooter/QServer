using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class SkillSteal : Skill
    {
        public override uint GetId()
        {
            return (uint)Items.SKILL_SKILLSTEAL;
        }
    }
}
