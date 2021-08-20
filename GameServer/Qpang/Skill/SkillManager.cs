using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class SkillManager
    {
        private Dictionary<uint, Skill> _skills;
        private Dictionary<byte, Dictionary<uint, Skill>> _skillsByMode;

        public SkillManager()
        {
            this._skillsByMode = new Dictionary<byte, Dictionary<uint, Skill>>();

            this._skills = new Dictionary<uint, Skill>()
            {
                //{ (uint)Items.SKILL_ABSORB, new Skills.Absorb() },
                { (uint)Items.SKILL_ASSASSIN, new Skills.Assassin() }, // OK
                //// { (uint)Items.SKILL_ASSASSIN2, new Skills.Assassin2() },
                //{ (uint)Items.SKILL_BLESS, new Skills.Bless() },
                //{ (uint)Items.SKILL_CALLGOZILLA, new Skills.CallGozilla() },
                //{ (uint)Items.SKILL_CALLWEAPON, new Skills.CallWeapon() },
                { (uint)Items.SKILL_CAMO, new Skills.Camo() },      // untested
                ////{ (uint)Items.SKILL_COUNTER, new Skills.Counter() },    // Unsupported
                //{ (uint)Items.SKILL_DEAL, new Skills.Deal() },
                ////{ (uint)Items.SKILL_DETECT, new Skills.Detect() }, // Unsupported i guess?
                //{ (uint)Items.SKILL_FLAGE, new Skills.Flage() },
                { (uint)Items.SKILL_HAWKEYE, new Skills.HawkEye() },        // untested
                //{ (uint)Items.SKILL_HIDING, new Skills.Hiding() },        // Counter Detect so probs unsuporrted too
                //{ (uint)Items.SKILL_INTERDICTION, new Skills.Interdiction() },
                ////{ (uint)Items.SKILL_INVINCIBLE, new Skills.Invincible() },      // Unsupported
                //{ (uint)Items.SKILL_IRONARM, new Skills.IronArm() },
                { (uint)Items.SKILL_IRONWALL, new Skills.IronWall() },
                //// { (uint)Items.SKILL_IRONWALL2, new Skills.IronWall2() },
                //{ (uint)Items.SKILL_JAMM, new Skills.Jamm() },
                //{ (uint)Items.SKILL_KINESIS, new Skills.Kinesis() },
                { (uint)Items.SKILL_POCKETSIZE, new Skills.PocketSize() }, // OK-ish
                //{ (uint)Items.SKILL_RAGE, new Skills.Rage() },
                //// { (uint)Items.SKILL_RAGE2, new Skills.Rage2() },
                //{ (uint)Items.SKILL_REPLAY, new Skills.Replay() },
                //// { (uint)Items.SKILL_REPLAY2, new Skills.Replay2() },
                { (uint)Items.SKILL_REVERS, new Skills.Revers() },  // OK
                //{ (uint)Items.SKILL_SBLCK, new Skills.Sblck() },
                //{ (uint)Items.SKILL_SHACKLES, new Skills.Shackles() },
                //{ (uint)Items.SKILL_SHIELD, new Skills.Shield() },
                //{ (uint)Items.SKILL_SHIELD2, new Skills.Shield2() },
                { (uint)Items.SKILL_SHOTMOVE, new Skills.ShotMove() },      // untested
                //// { (uint)Items.SKILL_SHOTMOVE2, new Skills.ShotMove2() },
                //{ (uint)Items.SKILL_SKILLTOUCH, new Skills.SkillTouch() },
                //{ (uint)Items.SKILL_SMOG, new Skills.Smog() },
                { (uint)Items.SKILL_STRONGSOUL, new Skills.StrongSoul() },      // Untested
                { (uint)Items.SKILL_STUNT, new Skills.Stunt() },
                //// { (uint)Items.SKILL_STUNT2, new Skills.Stunt2() },
                //{ (uint)Items.SKILL_TRANSE, new Skills.Transe() },
                { (uint)Items.SKILL_VITAL, new Skills.Vital() },        // OK
                //{ (uint)Items.SKILL_WBLCK, new Skills.Wblck() },
                //{ (uint)Items.SKILL_WEAPONCRACK, new Skills.WeaponCrack() },      // unsuported?
            };

            this._skillsByMode.Add((byte)GameMode.Mode.DM, this._skills);
            this._skillsByMode.Add((byte)GameMode.Mode.TDM, this._skills);

            this._skillsByMode.Add((byte)GameMode.Mode.PTE, this._skills);
            this._skillsByMode.Add((byte)GameMode.Mode.VIP, this._skills);

        }

        public Skill GetSkill(uint id)
        {
            if (this._skills.ContainsKey(id))
                return this._skills[id];
            return null;
        }

        public Dictionary<uint, Skill> GetSkillsForMode(byte mode)
        {
            if(this._skillsByMode.ContainsKey(mode))
                return this._skillsByMode[mode];
            return null;
        }
    }
}
