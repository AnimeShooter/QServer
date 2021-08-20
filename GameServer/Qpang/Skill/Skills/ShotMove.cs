using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class ShotMove : Skill
    {
        // Rapid firing rate 25% increase for gun

        public override uint GetId()
        {
            return (uint)Items.SKILL_SHOTMOVE;
        }
    }
}
