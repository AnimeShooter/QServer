using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer
{
    public static class Settings
    {
#if DEBUG
        public static uint SERVER_IP = BitConverter.ToUInt32(IPAddress.Parse("127.0.0.1").GetAddressBytes(), 0); // "127.0.0.1";
        public static uint SERVER_IP2 = BitConverter.ToUInt32(IPAddress.Parse("144.91.79.114").GetAddressBytes(), 0); // "127.0.0.1";
#else
        public static uint SERVER_IP = BitConverter.ToUInt32(IPAddress.Parse(Util.Util.GetLocalIPAddress().ToString()).GetAddressBytes(), 0);
#endif
        public static int SERVER_PORT_AUTH = 8003; // Auth
        public static int SERVER_PORT_LOBBY = 8005; // Park/Lobby
        public static int SERVER_PORT_SQUARE = 8012; // Square
        public static int SERVER_PORT_ROOM = 8020; // Game Room
        public static int HTTP_PORT_API = 8088; // REST API
        public static int WS_PORT = 8026; // Website

        // TODO: not hardcode?
        public static string ReCaptchaSitekey = "6LfAoqsbAAAAAKWJ-ifaAkQFLXFR9TABni9-ujYB";
        public static string ReCaptchaSecret = "6LfAoqsbAAAAAJon6b2LcRkoItnPMZH5c07nxAut";

#if DEBUG
        public static bool DEBUG = false;
#else
        public static bool DEBUG = false;
#endif

    }
}
