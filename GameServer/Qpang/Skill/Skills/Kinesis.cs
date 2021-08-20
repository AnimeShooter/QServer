using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Kinesis : Skill
    {
        // Move your character or an object by will of the mind for 30 sec

        public override uint GetId()
        {
            return (uint)Items.SKILL_KENESIS;
        }
    }
}
