using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Jamm : Skill
    {
        // Make the opponents team member look like your team member .

        public override uint GetId()
        {
            return (uint)Items.SKILL_JAMM;
        }

        public override uint GetDuration()
        {
            return 15;
        }
    }
}
