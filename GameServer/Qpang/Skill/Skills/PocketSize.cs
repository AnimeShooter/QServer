using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class PcketSize : Skill
    {
        // Reduce character size for 15 sec. HP 50% up. Speed 50% up
        public override uint GetId()
        {
            return (uint)Items.SKILL_POCKETSIZE;
        }
    }
}
