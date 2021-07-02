using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer
{
    public static class Settings
    {
        public static string SERVER_IP = "127.0.0.1";
        public static int SERVER_PORT_AUTH = 8003; // Auth
        public static int SERVER_PORT_PARK = 8005; // Park
        public static int SERVER_PORT_SQUARE = 8012; // Square
        public static int WS_PORT = 8826; // Website
    }
}
