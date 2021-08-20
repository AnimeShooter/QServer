using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class HawkEye : Skill
    {
        // Perfect Accuracy for 5 sec
        public override uint GetId()
        {
            return (uint)Items.SKILL_HAWKEYE;
        }
    }
}
