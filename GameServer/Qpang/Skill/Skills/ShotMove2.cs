using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class ShotMove2 : Skill
    {
        // Rapid rate 10% up

        public override uint GetId()
        {
            return (uint)Items.SKILL_SHOTMOVE2;
        }
    }
}
