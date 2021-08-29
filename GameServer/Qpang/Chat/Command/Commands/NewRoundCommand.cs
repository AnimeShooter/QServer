using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class NewRoundCommand : ChatCommand
    {
        public NewRoundCommand() : base(2)
        {

        }

        public override void Handle(Player player, List<string> args)
        {
            if (player == null || player.RoomPlayer == null)
                return;

            player.RoomPlayer.RoomSessionPlayer.Post(new GCPvENewRound());
        }
    }
}
