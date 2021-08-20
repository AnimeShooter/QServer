using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Interdiction : Skill
    {
        // Block all players including self
        public override uint GetId()
        {
            return (uint)Items.SKILL_INTERDICTION;
        }
    }
}
