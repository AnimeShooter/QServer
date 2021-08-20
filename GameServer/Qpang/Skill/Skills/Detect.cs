using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Detect : Skill
    {
        public override uint GetId()
        {
            return (uint)Items.SKILL_DETECT;
        }
    }
}
