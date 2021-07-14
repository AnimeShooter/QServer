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
            Log.Message(LogType.INIT, ">>        Starting GameServer      <<\n\n" +
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
            }

            // Auth Server
            ServerManager.AuthSession = new AuthServer();
            ServerManager.AuthSession.Server.Start();
            ServerManager.AuthSession.Server.StartConnectionThreads();

            // Init game server
            Game game = new Game();
            

            // Starting websocket
            NetServer wServer = new NetServer();
            new Thread(wServer.Start).Start();

#if DEBUG
            Log.Message(LogType.NORMAL, $"AuthServer listening on {Settings.SERVER_IP}:{Settings.SERVER_PORT_AUTH}");
            Log.Message(LogType.NORMAL, $"SqreServer listening on {Settings.SERVER_IP}:{Settings.SERVER_PORT_SQUARE}");
            Log.Message(LogType.NORMAL, $"ParkServer listening on {Settings.SERVER_IP}:{Settings.SERVER_PORT_PARK}");
            Log.Message(LogType.NORMAL, $"WSSocket   listening on {Settings.SERVER_IP}:{Settings.WS_PORT}\n");
#else
            Log.Message(LogType.NORMAL, $"AuthServer listening on {Util.Util.GetLocalIPAddress()}:{Settings.SERVER_PORT_AUTH}");
            Log.Message(LogType.NORMAL, $"SquareServer listening on {Util.Util.GetLocalIPAddress()}:{Settings.SERVER_PORT_SQUARE}");
            Log.Message(LogType.NORMAL, $"ParkServer listening on {Util.Util.GetLocalIPAddress()}:{Settings.SERVER_PORT_PARK}");
            Log.Message(LogType.NORMAL, $"WebSocket  listening on {Util.Util.GetLocalIPAddress()}:{Settings.WS_PORT}\n");
#endif

            HandlerMapping.InitPacketHandlers();

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

        }
    }
}
