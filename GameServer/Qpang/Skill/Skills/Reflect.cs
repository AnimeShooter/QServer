using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Reflect : Skill
    {
        // Reflects skills from opponents back onto them.

        public override uint GetId()
        {
            return (uint)Items.SKILL_REFLECT;
        }

        public override uint GetDuration()
        {
            return 30;
        }
    }
}
