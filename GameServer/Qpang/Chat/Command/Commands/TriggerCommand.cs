using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class TriggerCommand : ChatCommand
    {
        public TriggerCommand() : base(3)
        {

        }

        public override void Handle(Player player, List<string> args)
        {
            if (player == null || player.RoomPlayer == null)
                return;

            uint type = 0;
            byte state = 0;

            if (args.Count > 1)
                uint.TryParse(args[1], out type);

            if (args.Count > 2)
                byte.TryParse(args[2], out state);

            player.RoomPlayer.Room.RoomSession.RelayPlaying<GCPvEAreaTriggerInit>((uint)type, (uint)42369,
                new Spawn()
                {
                    X = player.RoomPlayer.RoomSessionPlayer.Position.X,
                    Y = player.RoomPlayer.RoomSessionPlayer.Position.Y,
                    Z = player.RoomPlayer.RoomSessionPlayer.Position.Z,
                }, state);
        }
    }
}
