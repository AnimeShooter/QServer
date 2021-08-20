using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Assassin2 : Skill
    {
        // Instant death with a back-stab attack

        public override uint GetId()
        {
            return (uint)Items.SKILL_ASSASSIN2;
        }
    }
}
