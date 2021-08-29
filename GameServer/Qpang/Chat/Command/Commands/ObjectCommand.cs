using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class ObjectCommand : ChatCommand
    {
        public ObjectCommand() : base(2)
        {

        }

        public override void Handle(Player player, List<string> args)
        {
            if (player == null || player.RoomPlayer == null)
                return;

            uint type = 0;
            ushort state = 0;

            if(args.Count > 1)
                uint.TryParse(args[1], out type);

            if (args.Count > 2)
                ushort.TryParse(args[2], out state);

            player.RoomPlayer.Room.RoomSession.RelayPlaying<GCPvEObjectInit>((uint)type, (uint)42169,
                new Spawn() { 
                    X = player.RoomPlayer.RoomSessionPlayer.Position.X, 
                    Y = player.RoomPlayer.RoomSessionPlayer.Position.Y,
                    Z = player.RoomPlayer.RoomSessionPlayer.Position.Z,
                }, state);

        }
    }
}
