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

        public RoomSkillManager()
        {
            
        }

        public void Initialize(RoomSession room)
        {
            this._room = room;
            this._skills = Game.Instance.SkillManager.GetSkillsForMode((byte)room.Room.Mode);
        }


        public Skill GenerateRandomSkill()
        {
            // Weird but works 
            Random rnd = new Random();
            int target = rnd.Next(0, this._skills.Count);

            var skills = this._skills.Values.ToList();

            Console.WriteLine("Someone looted: " + ((Items)skills[target].Id).ToString());

            // Note: Creates a new instance of the object
            return (Skill)Activator.CreateInstance(skills[target].GetType());
        }

        public bool IsSkillValid(uint itemId)
        {
            return this._skills.ContainsKey(itemId);
        }
    }
}
