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
        // TODO
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

            //string actualMessage = Game.Instance.ChatManager.
            // TODO
            //squarePlayer.Chat();
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

        // TODO
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
            packet.ReadUInt32();
            ushort character = packet.ReadUInt16();
            uint selectedWeapon = packet.ReadUInt32();
            uint[] equipment = new uint[9];
            for (int i = 0; i < equipment.Length; i++)
                equipment[i] = packet.ReadUInt32();

            var player = manager.Player;
            if (player == null)
                return;

            var squarePlayer = player.SquarePlayer;
            if (squarePlayer == null)
                return;

            squarePlayer.ChangeWeapon(selectedWeapon);
        }

        public static void HandleReloadSquareEvent(PacketReader packet, ConnServer manager)
        {
            var player = manager.Player;
            if (player == null)
                return;

            var squarePlayer = player.SquarePlayer;
            if (squarePlayer == null)
                return;

            manager.Send(Network.SquareManager.Instance.SetPosition(squarePlayer));
            squarePlayer.SetState(0);
        }

        public static void HandleRequestPlayers(PacketReader packet, ConnServer manager)
        {
            var player = manager.Player;
            if (player == null)
                return;

            var squarePlayer = player.SquarePlayer;
            if (squarePlayer == null)
                return;

            var square = squarePlayer.Square;
            var players = square.ListPlayers();
            manager.Send(SquareManager.Instance.Players(players, player.PlayerId));
        }

        public static void HandleUpdatePosition(PacketReader packet, ConnServer manager)
        {
            byte moveType = packet.ReadUInt8();
            byte direction = packet.ReadUInt8();
            float[] position = new float[3];
            for (int i = 0; i < position.Length; i++)
                position[i] = packet.ReadUInt8();

            var player = manager.Player;
            if (player == null || player.SquarePlayer == null)
                return;

            player.SquarePlayer.Move(position, direction, moveType);
        }

        public static void HandleUpdateStateEvent(PacketReader packet, ConnServer manager)
        {
            uint playerId = packet.ReadUInt32();
            uint state = packet.ReadUInt32();
            byte roomId = packet.ReadUInt8();

            var player = manager.Player;
            if (player == null || player.SquarePlayer == null)
                return;

            if (state == 7)
                player.SquarePlayer.SetState(7);
            else if (state == 5)
                player.SquarePlayer.SetState(5, roomId);
        }

    }
}
