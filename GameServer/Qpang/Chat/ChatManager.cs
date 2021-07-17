using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class ChatManager
    {
        private CommandManager commandManager;

        public CommandManager CommandManager
        {
            get { return this.commandManager; }
        }

        public ChatManager()
        {

            this.commandManager = new CommandManager();
        }

        public string Chat(Player player, string message)
        {
            if (message == "")
                return message;



            // TODO: check for commands?
            if(message[0] == '!')
            {
                //this.commandManager.
                return "";
            }

            return message;

        }
    }
}
