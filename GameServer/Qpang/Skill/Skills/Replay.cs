using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Replay : Skill
    {
        // Revive at respawn location instantly after death

        public override uint GetId()
        {
            return (uint)Items.SKILL_REPLAY;
        }
    }
}
