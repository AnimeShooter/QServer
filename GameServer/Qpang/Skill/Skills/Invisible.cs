using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Invisible : Skill
    {
        public override uint GetId()
        {
            return 0; // return (uint)Items.SKILL_INVISIBLE;
        }
    }
}
