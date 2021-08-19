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
            this._skills = new Dictionary<byte, Dictionary<uint, Skill>>();

            this._skills.Add((byte)GameMode.Mode.DM, new Dictionary<uint, Skill>()
            {
                { (uint)Items.SKILL_CAMO, new Camo() },
                { (uint)Items.SKILL_ASSASSIN, new Assassin() },
                { (uint)Items.SKILL_ABSORB, new Absorb() },
                { (uint)Items.SKILL_TEAM_CHEER, new TeamCheer() },
                { (uint)Items.SKILL_ZILLA, new Zilla() }
            });

        }

        public Dictionary<uint, Skill> GetSkillsForMode(byte mode)
        {
            if(this._skills.ContainsKey(mode))
                return this._skills[mode];
            return null;
        }
    }
}
