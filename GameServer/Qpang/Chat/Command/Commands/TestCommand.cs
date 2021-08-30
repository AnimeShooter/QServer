using System;
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

            uint type = 0;
            uint test = 0;

            if (args.Count > 1)
                uint.TryParse(args[1], out type);

            if (args.Count > 2)
                uint.TryParse(args[2], out test);

            switch(test)
            {
                case 1:
                 player.RoomPlayer.Room.RoomSession.RelayPlaying<GCPvEDestroyObject>(type);
                    break;
                case 2:
                    player.RoomPlayer.Room.RoomSession.RelayPlaying<GCPvEDoor>(type, true);
                    break;
                case 3:
                    player.RoomPlayer.Room.RoomSession.RelayPlaying<GCPvEDoor>(type, true);
                    break;
            }

        }
    }
}
