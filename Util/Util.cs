using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Qserver.Util
{
    public class Util
    {
        internal static IPAddress GetLocalIPAddress() // TODO remove debug
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string GenerateUUID()
        {
            Random rnd = new Random();
            string part1 = "";
            for (int i = 0; i < 6; i++)
                part1 += (char)rnd.Next(0x41, 0x5B);

            return $"{part1}-{rnd.Next(1000, 9999)}-{rnd.Next(1000, 0xFFFF).ToString("X4")}";
        }

        public static string GenerateToken()
        {
            Random rnd = new Random();
            string token = "";
            for(int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    switch(rnd.Next(0,3))
                    {
                        case 0: // upper
                            token += (char)rnd.Next(0x41, 0x5B);
                            break;
                        case 1: // lower
                            token += (char)rnd.Next(0x61, 0x7B);
                            break;
                        case 2: // numeric
                            token += (char)rnd.Next(0x30, 0x3A);
                            break;
                    }
                }
                    
                if (i < 4 - 1)
                    token += "-";
            }
            return token;
        }

        public static uint Timestamp(DateTime? time = null)
        {
            DateTime target = DateTime.UtcNow;
            if (time.HasValue)
                target = time.Value;

            return (uint)(target.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

    }
}
