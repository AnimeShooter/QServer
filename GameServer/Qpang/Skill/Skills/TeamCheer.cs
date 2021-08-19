using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class TeamCheer : Skill
    {
        public override uint GetId()
        {
            return (uint)Items.SKILL_TEAM_CHEER;
        }
    }
}
