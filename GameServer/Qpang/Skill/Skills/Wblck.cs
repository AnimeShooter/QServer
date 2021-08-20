using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Wblck : Skill
    {
        // Make others unable to change their weapons

        public override uint GetId()
        {
            return (uint)Items.SKILL_WBLCK;
        }
    }
}
