using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Hiding : Skill
    {
        // Preventing a skill card of "Finding Opponent"
        public override uint GetId()
        {
            return (uint)Items.SKILL_HIDING;
        }
    }
}
