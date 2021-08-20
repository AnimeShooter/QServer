using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Rage2 : Skill
    {
        // Target must be in the Cross Hairs to use this skill; Cross Hairs 75% up for 15 sec. (Accuracy will Drop.)
        public override uint GetId()
        {
            return (uint)Items.SKILL_ABSORB;
        }
    }
}
