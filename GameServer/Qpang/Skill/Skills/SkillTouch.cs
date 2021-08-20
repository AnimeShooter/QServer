using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class SkillTouch : Skill
    {
        // Switch equiped weapons quickly

        public override uint GetId()
        {
            return (uint)Items.SKILL_SKILLTOUCH;
        }
    }
}
