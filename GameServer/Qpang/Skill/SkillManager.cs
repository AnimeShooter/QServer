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

            var allSkills = new Dictionary<uint, Skill>()
            {
                { (uint)Items.SKILL_ABSORB, new Skills.Absorb() },
                { (uint)Items.SKILL_ANGER, new Skills.Anger() },
                { (uint)Items.SKILL_ASSASSIN, new Skills.Assassin() },
                { (uint)Items.SKILL_CAMO, new Skills.Camo() },
                { (uint)Items.SKILL_CHAOS, new Skills.Chaos() },
                { (uint)Items.SKILL_ENERGY, new Skills.Energy() },
                // { (uint)Items.SKILL_INVISIBLE, new Skills.Invisible() },
                { (uint)Items.SKILL_RAPIDFIRE, new Skills.RapidFire() },
                { (uint)Items.SKILL_SKILLSTEAL, new Skills.SkillSteal() },
                { (uint)Items.SKILL_TEAM_CHEER, new Skills.TeamCheer() },
                { (uint)Items.SKILL_TRADE, new Skills.Trade() },
                { (uint)Items.SKILL_TRAP, new Skills.Trap() },
                { (uint)Items.SKILL_UNDERCOVER, new Skills.Undercover() },
                { (uint)Items.SKILL_WEAPONSTEAL, new Skills.WeaponSteal() },
                { (uint)Items.SKILL_ZILLA, new Skills.Zilla() }
            };

            this._skills.Add((byte)GameMode.Mode.DM, allSkills);
            this._skills.Add((byte)GameMode.Mode.TDM, allSkills);

            this._skills.Add((byte)GameMode.Mode.PTE, allSkills);
            this._skills.Add((byte)GameMode.Mode.VIP, allSkills);

        }

        public Dictionary<uint, Skill> GetSkillsForMode(byte mode)
        {
            if(this._skills.ContainsKey(mode))
                return this._skills[mode];
            return null;
        }
    }
}
