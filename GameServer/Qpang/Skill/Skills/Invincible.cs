using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Invincible : Skill
    {
        // Invincible for 10 sec
        public override uint GetId()
        {
            return (uint)Items.SKILL_INVINCIBLE;
        }
    }
}
