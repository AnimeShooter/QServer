using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Smog : Skill
    {
        // If HP drops under 25%; a smoke screen will protect you within 2M radius for 15 sec

        public override uint GetId()
        {
            return (uint)Items.SKILL_SMOG;
        }
    }
}
