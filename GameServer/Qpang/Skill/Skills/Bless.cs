using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Bless : Skill
    {
        // Grants Energy shield to entire team. 

        public override uint GetId()
        {
            return (uint)Items.SKILL_BLESS;
        }
    }
}
