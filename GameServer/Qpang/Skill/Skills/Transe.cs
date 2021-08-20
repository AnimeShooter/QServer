using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Transe : Skill
    {
        // haracter will transform into Battery Man

        public override uint GetId()
        {
            return (uint)Items.SKILL_TRANSE;
        }
    }
}
