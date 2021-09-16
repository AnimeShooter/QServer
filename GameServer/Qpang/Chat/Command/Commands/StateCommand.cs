using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class StateCommand : ChatCommand
    {
        public StateCommand() : base(2)
        {

        }

        public override void Handle(Player player, List<string> args)
        {
            if (player == null || player.RoomPlayer == null)
                return;

            uint cmd = 0;
            uint cmd2 = 0;
            uint cmd3 = 0;

            if(args.Count > 1)
                uint.TryParse(args[1], out cmd);

            if (args.Count > 2)
                uint.TryParse(args[2], out cmd2);

            if (args.Count > 3)
                uint.TryParse(args[3], out cmd3);

            if (cmd == 36)
                player.RoomPlayer.Room.RoomSession.PublicEnemy = player.RoomPlayer.RoomSessionPlayer;

            player.RoomPlayer.RoomSessionPlayer.Post(new GCGameState(player.PlayerId, cmd, cmd2, cmd3));
        }
    }
}
