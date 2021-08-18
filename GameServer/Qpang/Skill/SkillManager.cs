using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class SkillManager
    {
        private Dictionary<byte, Dictionary<uint, Skill>> _skills;

        public SkillManager()
        {

        }

        public void Initialize()
        {
            this._skills = new Dictionary<byte, Dictionary<uint, Skill>>();

            var dm = this._skills[(byte)GameMode.Mode.DM];
            dm.Add((uint)Items.SKILL_CAMO, new Camo());
            dm.Add((uint)Items.SKILL_ASSASSIN, new Assassin());
            dm.Add((uint)Items.SKILL_ABSORB, new Absorb());
            dm.Add((uint)Items.SKILL_TEAM_CHEER, new TeamCheer());
            dm.Add((uint)Items.SKILL_ZILLA, new Zilla());
        }

        public Dictionary<uint, Skill> GetSkillsForMode(byte mode)
        {
            if(this._skills.ContainsKey(mode))
                return this._skills[mode];
            return null;
        }
    }
}
