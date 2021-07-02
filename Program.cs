using System;
using System.Threading;
using Qserver.GameServer;
using Qserver.Util;

namespace Qserver
{
    class Program
    {
        static void Main(string[] args)
        { 
            var crccc = test.crc32(new byte[] { 0x26, 0x01, 0x8A, 0x26, 0x2F, 0x34, 0xC3, 0x2A, 0x4E, 0x1A, 0x05, 0x88 });

            LaunchServer();
            while (true)
            {
                Console.Write("- [1]: Launch Server\nSelect option: ");
                var key = Console.ReadKey();
                Console.WriteLine();


                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        LaunchServer();
                        break;
                    default:
                        Console.WriteLine("Option invalied!");
                        break;
                }
            }
                
        }

        private static void LaunchServer()
        {
            GameServer.GameServer.Start();
        }
    }
}
