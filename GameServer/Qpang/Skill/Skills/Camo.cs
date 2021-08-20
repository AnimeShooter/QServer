using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Camo : Skill
    {
        // Make yourself invisible. You can move around but when you get an item| attacked| or roll the skill will be canceled. 

        public override uint GetId()
        {
            return (uint)Items.SKILL_CAMO;
        }
    }
}
