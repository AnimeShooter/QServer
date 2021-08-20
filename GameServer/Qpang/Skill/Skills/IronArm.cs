using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class IronArm : Skill
    {
        // Throwing weapon's speed 2 times faster OR Word een wandelende muur. Werkt 20 seconden.
        public override uint GetId()
        {
            return (uint)Items.SKILL_IRONARM;
        }
    }
}
