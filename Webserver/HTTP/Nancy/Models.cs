using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.Webserver.HTTP.Nancy
{
    public struct APIResponse<T>
    {
        public T Result;
        public string Message;
    }

    // TODO: Place elsewhere?
    public class SkillCard
    {
        public string CodeName;
        public string Name;
        public string Description;
        public uint ItemId;

        public byte Type; // (copper?) bronze, silver, gold, rainbow 
        public byte Points;
        public byte TargetType; // self, player, team?
        public byte Duration;

        public string Texture; // itemId
    }
}
