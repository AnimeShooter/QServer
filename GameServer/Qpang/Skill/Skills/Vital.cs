using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Vital : Skill
    {
        // Use this to sacrifice your HP (reduce your HP to 1)  and regenerate  members HP 5 points per second for a certain time. 

        public override uint GetId()
        {
            return (uint)Items.SKILL_VITAL;
        }
    }
}
