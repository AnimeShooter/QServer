using System;
using System.Threading;
using Qserver.GameServer;
using Qserver.Util;
using Qserver.GameServer.Qpang;
using Qserver.Database.Repositories;
using Qserver.Database;

namespace Qserver
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServer.GameServer.Start(args);
        }
    }
}
