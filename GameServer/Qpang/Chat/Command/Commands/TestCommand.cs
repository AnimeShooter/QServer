﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class TestCommand : ChatCommand
    {
        public TestCommand() : base(3)
        {

        }

        public override void Handle(Player player, List<string> args)
        {
            if (player == null || player.RoomPlayer == null)
                return;

            uint test1 = 0;
            uint test2 = 0;
            uint test3 = 0;

            if (args.Count > 1)
                uint.TryParse(args[1], out test1);

            if (args.Count > 2)
                uint.TryParse(args[2], out test2);

            if (args.Count > 2)
                uint.TryParse(args[2], out test3);

            //switch(test)
            //{
            //    case 1:
            //     player.RoomPlayer.Room.RoomSession.RelayPlaying<GCPvEDestroyObject>(type);
            //        break;
            //    case 2:
            //        player.RoomPlayer.Room.RoomSession.RelayPlaying<GCPvEDoor>(type, true);
            //        break;
            //    case 3:
            //        player.RoomPlayer.Room.RoomSession.RelayPlaying<GCPvEDoor>(type, true);
            //        break;
            //}

            //player.RoomPlayer.RoomSessionPlayer.WeaponManager.Replace(Game.Instance.WeaponManager.GetRandomWeapon());
            //player.RoomPlayer.RoomSessionPlayer.Post(new GCGameState(player.RoomPlayer.RoomSessionPlayer, 15)); // leave game
            //player.RoomPlayer.RoomSessionPlayer.Post(new GCStart(player.RoomPlayer.Room, player.PlayerId)); // start room
            //player.RoomPlayer.RoomSessionPlayer.Post(new GCJoin(player.RoomPlayer.RoomSessionPlayer)); // join game
            //player.RoomPlayer.RoomSessionPlayer.Post(new GCGameState(player.RoomPlayer.RoomSessionPlayer, 12)); // game state

            player.RoomPlayer.RoomSessionPlayer.Post(new GCPvEMoveNpc(test1, (ushort)test2, (ushort)test3)); // weapon
        }
    }
}
