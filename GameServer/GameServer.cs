using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Util;
using Qserver.GameServer.Network;
using Qserver.GameServer.Packets;
using System.Threading;
using Qserver.External.Websocket;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer
{
    public static class GameServer
    {
        // ### Patch Client ###
        // 040114A: push 0x90000000 -> 0x100800000 [68 00 00 08 10]
        // 042DBC9: jmp [EB 14 90 90 90 90] // removes anoying hook
        //
        //

        public static void PrintBanner()
        {
            Log.Message(LogType.MISC, "Starting GameServer\n\n" +
                                      "                   ^		            \n" +
                                      "                  / \\			    \n" +
                                      "                 /   \\			    \n" +
                                      "                /   Essence		    \n" +
                                      "               /       Emulator	    \n" +
                                      "              /         \\		    \n" +
                                      "              \\         /	        \n" +
                                      "           By: \\       /     	    \n" +
                                      "              Dennis &/       	    \n" +
                                      "                 Deluze &   	        \n" +
                                      "                  \\ / Ferib 	    \n" +
                                      "                   v     		    \n");
        }

        public static void PrintHelp()
        {
            Log.Message(LogType.CLI, $"{"--Help, -h".PadRight(20)}: Prints the Help menu\n" +
                                     $"{"--NoAuth".PadRight(20)}: Exclude AuthServer\n" +
                                     $"{"--NoGameServer, --NoGame".PadRight(20)}: Exclude GameServer\n" +
                                     $"{"--NoSquare".PadRight(20)}: Exclude SquareServer\n" +
                                     $"{"--NoLobby".PadRight(20)}: Exclude LobbyServer\n" +
                                     $"{"--WebSocket".PadRight(20)}: Inlcude WebSocket\n");
        }

        public static void Start(string[] args)
        {
            PrintBanner();

            if (args.Length > 2 && (args[1].Equals("-h", StringComparison.OrdinalIgnoreCase) || args[1].Equals("--help", StringComparison.OrdinalIgnoreCase)))
            {
                PrintHelp();
                return;
            }

            // settings
            bool startAuthServer = true;
            bool startSquareServer = true;
            bool startLobbyServer = true;
            bool startWebsocketServer = false;
            //bool startGameServer = true;

            foreach(var arg in args)
            {
                if (arg.Equals("--NoAuth", StringComparison.OrdinalIgnoreCase))
                    startAuthServer = false;
                else if (arg.Equals("--NoSquare", StringComparison.OrdinalIgnoreCase))
                    startSquareServer = false;
                 else if (arg.Equals("--NoLobby", StringComparison.OrdinalIgnoreCase))
                    startLobbyServer = false;
                 else if (arg.Equals("--WebSocket", StringComparison.OrdinalIgnoreCase))
                    startWebsocketServer = true;
                else if (arg.Equals("--NoGameServer", StringComparison.OrdinalIgnoreCase) || arg.Equals("--NoGame", StringComparison.OrdinalIgnoreCase))
                {
                    startLobbyServer = false;
                    startSquareServer = false;
                }
            }

            // Auth Server
            if (startAuthServer)
            {
                ServerManager.AuthSession = new AuthServer();
                ServerManager.AuthSession.Server.Start();
                ServerManager.AuthSession.Server.StartConnectionThreads();
                Log.Message(LogType.NORMAL, $"AuthServer    listening on {Settings.SERVER_IP}:{Settings.SERVER_PORT_AUTH}");
            }

            // Init game server (Square + Lobby)
            if(startSquareServer || startLobbyServer) // Square always on?
            {
                Game game = new Game(startLobbyServer);
                if(startSquareServer)
                    Log.Message(LogType.NORMAL, $"SquareServer  listening on {Settings.SERVER_IP}:{Settings.SERVER_PORT_SQUARE}");
                if(startLobbyServer)
                    Log.Message(LogType.NORMAL, $"LobbyServer   listening on {Settings.SERVER_IP}:{Settings.SERVER_PORT_PARK}");
            }

            // Starting websocket
            if (startWebsocketServer) // DEFAULT: FALSE !!
            {
                NetServer wServer = new NetServer();
                new Thread(wServer.Start).Start();
                Log.Message(LogType.NORMAL, $"WebSocket     listening on {Settings.SERVER_IP}:{Settings.WS_PORT}\n");
            }

            HandlerMapping.InitPacketHandlers(startAuthServer, startLobbyServer, startSquareServer);

            GC.Collect();
            Log.Message(LogType.NORMAL, $"Total Memory: {Convert.ToSingle(GC.GetTotalMemory(false) / 1024 / 1024)}MB");
            Log.Message(LogType.CLI,$"CLI Ready.\n{new string('=', Console.WindowWidth)}\n" + Commands.Help() + $"{new string('=', Console.WindowWidth)}");

            while (true)
            {
                var old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(Commands.ExecuteCommand(Console.ReadLine()));
                Console.ForegroundColor = old;
            }
            // TODO: listen on console
        }
    }
}
