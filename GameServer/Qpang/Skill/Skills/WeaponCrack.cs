using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class WeaponCrack : Skill
    {
        // Drop a weapon at a certain rate

        public override uint GetId()
        {
            return (uint)Items.SKILL_WEAPONCRACK;
        }
    }
}
