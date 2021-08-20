using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Replay2 : Skill
    {
        // Revive at respawn location instantly after death
        public override uint GetId()
        {
            return (uint)Items.SKILL_REPLAY2;
        }
    }
}
