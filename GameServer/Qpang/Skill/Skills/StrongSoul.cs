using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class StrongSoul : Skill
    {
        // Beschermt tegen explosieschade. / Block stun or speed down once

        public override uint GetId()
        {
            return (uint)Items.SKILL_STRONGSOUL;
        }
    }
}
