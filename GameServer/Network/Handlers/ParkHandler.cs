using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Util;
using Qserver.GameServer.Qpang;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Managers;
using Qserver.GameServer.Network.Packets;
using System.Threading;

namespace Qserver.GameServer.Network.Handlers
{
    public class ParkHandler
    {
        public static void HandleLobbyLogin(PacketReader packet, ConnServer manager)
        {
            byte[] uuid = packet.ReadBytes(16);

            uint userId = 1;
            // databse UUID to ID

            bool isBanned = false;
            if(isBanned)
            {
                manager.Send(ParkManager.Instance.Banned());
                manager.CloseSocket();
                return;
            }

            var player = Game.Instance.CreatePlayer(manager, userId);

            manager.Send(ParkManager.Instance.Authenticated(player));
        }

    }
}
