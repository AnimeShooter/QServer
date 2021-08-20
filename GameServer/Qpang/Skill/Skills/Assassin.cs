using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Assassin : Skill
    {
        // One hit kill with melee weapon. When done successfully skill will disappear.

        public override uint GetId()
        {
            return (uint)Items.SKILL_ASSASSIN;
        }

        public override uint GetDuration()
        {
            return 99;
        }
    }
}
