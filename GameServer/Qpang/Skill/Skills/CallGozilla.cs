using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class CallGozilla : Skill
    {
        // Character will transform into a Giant Beast.

        public override uint GetId()
        {
            return (uint)Items.SKILL_CALLGOZILLA;
        }
    }
}
