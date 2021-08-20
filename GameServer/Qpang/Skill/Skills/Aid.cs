using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Aid : Skill
    {
        // All allies gain unlimited stamina for 15 secs. Halves stamina after the duration.

        public override uint GetId()
        {
            return (uint)Items.SKILL_AID;
        }
    }
}
