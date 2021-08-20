using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Stunt2 : Skill
    {
        // Deal 25% damage during action

        public override uint GetId()
        {
            return (uint)Items.SKILL_STUNT2;
        }
    }
}
