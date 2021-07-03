using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Util;
using Qserver.GameServer.Network;
using Qserver.GameServer.Packets;
using System.Threading;
using Qserver.External.Websocket;

namespace Qserver.GameServer
{
    public static class GameServer
    {
        public static void Start()
        {
            Log.Message(LogType.INIT, ">> Starting GameServer <<");

            //Settings.Init();

            ServerManager.ServerSession = new ServerSocket();
            //ServerManager.ServerSession = new ParkSocket();
            if(!ServerManager.ServerSession.Start())
            {
                Log.Message(LogType.ERROR, "GameServer failed to start");
                return;
            }

            // Starting websocket
            NetServer wServer = new NetServer();
            new Thread(wServer.Start).Start();

            // Starting game server
            ServerManager.ServerSession.StartConnectionThreads();
#if DEBUG
            Log.Message(LogType.NORMAL, $"AuthServer listening on {Settings.SERVER_IP}:{Settings.SERVER_PORT_AUTH}");
            Log.Message(LogType.NORMAL, $"SquareServer listening on {Settings.SERVER_IP}:{Settings.SERVER_PORT_SQUARE}");
            Log.Message(LogType.NORMAL, $"ParkServer listening on {Settings.SERVER_IP}:{Settings.SERVER_PORT_PARK}");
            Log.Message(LogType.NORMAL, $"WebSocket  listening on {Settings.SERVER_IP}:{Settings.WS_PORT}\n");
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
