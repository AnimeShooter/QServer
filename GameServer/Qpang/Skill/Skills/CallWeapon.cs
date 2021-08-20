using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class CallWeapon : Skill
    {
        // Character will transform into the Last Weapon
        public override uint GetId()
        {
            return (uint)Items.SKILL_CALLWEAPON;
        }
    }
}
