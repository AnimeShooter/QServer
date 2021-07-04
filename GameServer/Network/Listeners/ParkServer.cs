using System;
using System.Net.Sockets;
using System.Threading;
using Qserver.Util;
using Qserver.GameServer.Network.Packets;
using Qserver.GameServer.Packets;
using Qserver.GameServer.Network.Managers;
using System.Threading.Tasks;
using Qserver.GameServer.Helpers;
using Qserver.GameServer.Network;

namespace Qserver.GameServer.Network
{
    public class ParkServer : ConnServer
    {
        public ParkServer()
        {
            Server = new QpangServer(Settings.SERVER_PORT_PARK);
        }

    }
}
