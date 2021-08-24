﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class DummyCommand : ChatCommand
    {
        public DummyCommand() : base(3)
        {

        }

        public override void Handle(Player player, List<string> args)
        {
            if (player == null || player.RoomPlayer == null || player.RoomPlayer.RoomSessionPlayer == null)
                return;

            byte team = 0;
            if (player.RoomPlayer.Team == 1)
                team = 2;
            else if (player.RoomPlayer.Team == 2)
                team = 1;

            Random rnd = new Random();
            var conn = new GameConnection();

            conn.Player = new Player("-BOT");
            conn.Player.RoomPlayer = new RoomPlayer(conn, player.RoomPlayer.Room);
            player.RoomPlayer.Room.RoomSession.AddPlayer(conn, team);
        }
    }
}
