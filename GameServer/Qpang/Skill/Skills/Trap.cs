using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Trap : Skill
    {
        public override uint GetId()
        {
            return (uint)Items.SKILL_TRAP;
        }
    }
}
