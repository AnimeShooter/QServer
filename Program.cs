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
