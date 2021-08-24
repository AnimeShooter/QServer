using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class StartCommand : ChatCommand
    {
        public StartCommand() : base(3)
        {

        }

        public override void Handle(Player player, List<string> args)
        {
            if (player == null || player.RoomPlayer == null)
                return;

            if (!player.RoomPlayer.Room.Playing)
                player.RoomPlayer.Room.Start(player);
        }
    }
}
