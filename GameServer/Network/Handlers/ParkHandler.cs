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

        #region Channel
        public static void HandleChannelList(PacketReader packet, ConnServer manager)
        {
            var list = new List<Channel>();
            list.Add(new Channel()
            {
                CurrPlayers = 59,
                MaxLevel = 99,
                MinLevel = 0,
                MaxPlayers = 120,
                Name = "Test",
                Id = 1
            });
            manager.Send(ParkManager.Instance.ChannelList(list));
        }
        public static void HandleChannelHost(PacketReader packet, ConnServer manager)
        {
            uint channelId = packet.ReadUInt32();
            manager.Send(ParkManager.Instance.ChannelHost(channelId));
        }
        #endregion Channel

    }
}
