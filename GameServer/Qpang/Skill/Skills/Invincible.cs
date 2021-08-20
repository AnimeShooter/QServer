using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Invincible : Skill
    {
        // Invincible for 10 sec
        public override uint GetId()
        {
            return (uint)Items.SKILL_INVINCIBLE;
        }

        public override uint GetDuration()
        {
            return 10;
        }

        public override void OnUse(RoomSessionPlayer target)
        {
            target.MakeInvincible(GetDuration());

            base.OnUse(target);
        }
    }
}
