using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class SpawnCommand : ChatCommand
    {
        public SpawnCommand() : base(2)
        {

        }

        public override void Handle(Player player, List<string> args)
        {
            if (player == null || player.RoomPlayer == null)
                return;

            uint type = 0;

            if(args.Count > 1)
                uint.TryParse(args[1], out type);

            player.RoomPlayer.Room.RoomSession.RelayPlaying<GCPvENpcInit>((uint)type, (uint)42069, (ushort)1, (byte)1, 
                new Position() { 
                    X = player.RoomPlayer.RoomSessionPlayer.Position.X, 
                    Y = player.RoomPlayer.RoomSessionPlayer.Position.Y,
                    Z = player.RoomPlayer.RoomSessionPlayer.Position.Z,
                });

        }
    }
}
