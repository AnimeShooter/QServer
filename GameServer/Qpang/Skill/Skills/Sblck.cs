using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Sblck : Skill
    {
        // Make others unable to use their skills
        public override uint GetId()
        {
            return (uint)Items.SKILL_SBLCK;
        }
    }
}
