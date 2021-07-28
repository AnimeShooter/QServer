using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class RoomSkillManager
    {
        private RoomSession _room;
        private Dictionary<uint, Skill> _skills;

        public RoomSkillManager(RoomSession room)
        {
            this._room = room;
            //this._skills = Game.Instance.SkillManager.SkillsGotMode(room.Room.Mode); // TODO
        }

        public Skill GenerateRandomSkill()
        {
            // idk
            return null;
        }

        public bool IsSkillValid(uint itemId)
        {
            return this._skills.ContainsKey(itemId);
        }
    }
}
