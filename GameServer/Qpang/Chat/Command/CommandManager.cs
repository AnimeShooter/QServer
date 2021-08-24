using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class CommandManager
    {
        private Dictionary<string, ChatCommand> _commands = new Dictionary<string, ChatCommand>()
        {
            {"start", new StartCommand() },
            {"dummy", new DummyCommand() }
        };

        public CommandManager()
        {

        }

        public bool IsCommand(string str)
        {
            return this._commands.ContainsKey(str);
        }

        public void Handle(Player player, string cmd, string message)
        {
            if (!this._commands.ContainsKey(cmd))
                return;

            var handler = this._commands[cmd];

            if (!handler.CanHandle(player))
                return;

            handler.Handle(player, null);
        }
    }
}
