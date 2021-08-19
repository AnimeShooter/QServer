using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class WeaponSteal : Skill
    {
        public override uint GetId()
        {
            return (uint)Items.SKILL_WEAPONSTEAL;
        }
    }
}
