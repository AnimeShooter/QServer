﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer
{
    public static class Settings
    {
#if DEBUG
        public static string SERVER_IP = "127.0.0.1";
#else
        public static string SERVER_IP = Util.Util.GetLocalIPAddress().ToString();
#endif
        public static int SERVER_PORT_AUTH = 8003; // Auth
        public static int SERVER_PORT_LOBBY = 8005; // Park/Lobby
        public static int SERVER_PORT_SQUARE = 8012; // Square
        public static int SERVER_PORT_ROOM = 8020; // Game Room
        public static int WS_PORT = 8826; // Website

#if DEBUG
        public static bool DEBUG = true;
#else
        public static bool DEBUG = false;
#endif

    }
}
