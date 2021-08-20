using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class IronWall : Skill
    {
        // Be a moving wall for 15/20 seconds. You can not attack when this skill is active.
        public override uint GetId()
        {
            return (uint)Items.SKILL_IRONWALL;
        }
    }
}
