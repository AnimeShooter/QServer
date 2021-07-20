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
        
    }
}
