using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class ChatManager
    {
        private CommandManager _commandManager;

        public CommandManager CommandManager
        {
            get { return this._commandManager; }
        }

        public ChatManager()
        {

            this._commandManager = new CommandManager();
        }

        public string Chat(Player player, string message)
        {
            if (message == "")
                return message;

            if(message[0] == '!')
            {
                string cmd = message.Split(' ')[0];
                cmd = cmd.Substring(1);
                if (this._commandManager.IsCommand(cmd))
                {
                    this._commandManager.Handle(player, cmd, message);
                    return "";
                }
            }

            return message;
        }
    }
}
