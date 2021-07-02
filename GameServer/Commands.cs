using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Util;
using Qserver.GameServer.Singleton;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Managers;
using Qserver.GameServer.Packets;

namespace Qserver.GameServer
{
    public static class Commands
    {
        private static string[] CmdName = new string[]
        { 
            "Help",
            "Info",
            "Target",
            "Mute",
            "Players",
            "Chat",
            "Exit"
        };
        private static string[] CmdInfo = new string[] 
        { 
            "Show Help",
            "Show Server Info",
            "Set Unit Target",
            "Toggle mute/unmute packet info",
            "Show all online Players",
            "Send a chat message to all players",
            "Exits the server"
        };

        private static uint TargetGuid;

        public static string ExecuteCommand(string command)
        {
            return "";
        }

        public static string Help()
        {
            string output = "";

            for(int i = 0; i < CmdName.Length && i < CmdInfo.Length; i++)
            {
                output += $"[-] {CmdName[i].PadRight(25)}: {CmdInfo[i]}\n";
            }

            return output;
        }

        #region commandHandlers

        #endregion
    }
}
