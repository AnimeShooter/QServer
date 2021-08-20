using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Shield2 : Skill
    {
        // Deploy an invisible shield for 20 sec.  Can not shield short range; back and splash damage

        public override uint GetId()
        {
            return (uint)Items.SKILL_SHIELD2;
        }
    }
}
