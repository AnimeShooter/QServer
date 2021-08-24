using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class ChatCommand
    {
        public enum Validation : byte
        {
            STRING,
            INTEGER
        }

        private byte _rank;
        public Dictionary<byte, CommandArgument> ArgTypes = new Dictionary<byte, CommandArgument>()
        {
            //{ChatCommand.Validation.STRING, new StringArgument() },
            //{ChatCommand.Validation.STRING, new StringArgument() },
        };

        public  ChatCommand(byte rank)
        {
            this._rank = rank;
        }

        public virtual bool CanHandle(Player player)
        {
            return player.Rank >= this._rank;
        }

        public virtual void Handle(Player player, List<string> args)
        {
            
        }
    }
}
