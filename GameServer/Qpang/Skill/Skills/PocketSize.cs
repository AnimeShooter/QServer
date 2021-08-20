using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class PocketSize : Skill
    {
        // Reduce character size for 15 sec. HP 50% up. Speed 50% up
        public override uint GetId()
        {
            return (uint)Items.SKILL_POCKETSIZE;
        }

        public override uint GetDuration()
        {
            return 15;
        }

        public override void OnUse(RoomSessionPlayer target)
        {
            target.SetHealth((ushort)((target.Health + target.BonusHealth) * 1.5f), true);

            base.OnUse(target);
        }
    }
}
