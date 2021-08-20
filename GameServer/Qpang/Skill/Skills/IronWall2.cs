using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class IronWall2 : Skill
    {
        //  Be a moving wall for 15 sec
        public override uint GetId()
        {
            return (uint)Items.SKILL_IRONWALL2;
        }
    }
}
