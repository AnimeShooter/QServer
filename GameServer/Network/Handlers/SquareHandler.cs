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
    public class SquareHandler
    {
        public static void HandleChatRequest(PacketReader packet, ConnServer manager)
        {
            packet.ReadBytes(34);
            ushort len = packet.ReadUInt16();
            string msg = packet.ReadWString(len & 254); // prevent over 255

            Player player = manager.Player;
            if (player == null)
                return;

            SquarePlayer squarePlayer = player.SquarePlayer;
            if (squarePlayer == null)
                return;

            // TODO
            throw new NotImplementedException();

        }

        public static void HandleConnectRequest(PacketReader packet, ConnServer manager)
        {
            uint playerId = packet.ReadUInt32();
            string nickname = packet.ReadWString(16);

            Player player = Game.Instance.GetPlayer(playerId);
            if (player == null)
                return;

            player.SquareConnection = manager;
            manager.Player = player;

            // squares
            var squares = Game.Instance.SquareManager.List();
            manager.Send(SquareManager.Instance.SquareList(squares));
        }

        public static void HandleEmoteEevent(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }

        public static void HandleSquareLogin(PacketReader packet, ConnServer manager) // JoinSquare
        {
            uint playerId = packet.ReadUInt32();
            uint squareId = packet.ReadUInt32();

            Player player = manager.Player;

            if (player == null)
                return;

            bool isInSquare = player.SquarePlayer != null;

            if (isInSquare)
            {
                player.SquarePlayer.Square.Remove(player.PlayerId);
                player.LeaveSquare();

                var square = Game.Instance.SquareManager.Get(squareId);
                if (square == null)
                    return;

                square.Add(player);
            }
            else
            {
                var square = Game.Instance.SquareManager.GetAvailableSquare();
                square.Add(player);
            }

        }

        public static void HandleLeftInventory(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }

        public static void HandleReloadSquareEvent(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }

        public static void HandleRequestPlayers(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }

        public static void HandleUpdatePosition(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }

        public static void HandleUpdateStateEvent(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }

       
    }
}
