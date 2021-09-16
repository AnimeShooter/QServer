using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class CommandManager
    {
        private Dictionary<string, ChatCommand> _commands = new Dictionary<string, ChatCommand>()
        {
            {"start", new StartCommand() },
            {"dummy", new DummyCommand() },
            {"EndRound", new EndRoundCommand() },
            {"NewRound", new NewRoundCommand() },
            {"EndGame", new EndGameCommand() },
            {"spawn", new SpawnCommand() },
            {"object", new ObjectCommand() },
            {"item", new ItemCommand() },
            {"trigger", new TriggerCommand() },
            {"test", new TestCommand() },
            {"state", new StateCommand() }
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

            List<string> args = message.Split(' ').ToList();
            handler.Handle(player, args);
        }
    }
}
