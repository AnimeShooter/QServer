using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Skills
{
    public class Deal : Skill
    {
        // Sacrifice all your skill points to reduce opponents skill points by 1 (applies to whole team)  

        public override uint GetId()
        {
            return (uint)Items.SKILL_DEAL;
        }

        public override uint GetDuration()
        {
            return 0;
        }

        public override void OnUse(RoomSessionPlayer target)
        {
            List<RoomSessionPlayer> enemies;
            if (target.Team == 2)
                enemies = target.RoomSession.GetPlayersForTeam(1);
            else
                enemies = target.RoomSession.GetPlayersForTeam(2);

            foreach (var e in enemies)
                e.SkillManager.RemoveSkillPoints(100);

            // Dont do base?
            //base.OnUse(target);
        }
    }
}
