using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Vital : Skill
    {
        // Use this to sacrifice your HP (reduce your HP to 1)  and regenerate  members HP 5 points per second for a certain time. 

        public override uint GetId()
        {
            return (uint)Items.SKILL_VITAL;
        }

        public override uint GetDuration()
        {
            return 10;
        }

        public override void OnUse(RoomSessionPlayer target)
        {
            // target is self?
            target.TakeHealth((ushort)(target.Health - 1), true);

            base.OnUse(target);
        }
    }
}
