using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class SkillBook : GameItem
    {
        public override uint OnPickUp(RoomSessionPlayer session)
        {
            return session.SkillManager.DrawSkill();
        }
    }
}
