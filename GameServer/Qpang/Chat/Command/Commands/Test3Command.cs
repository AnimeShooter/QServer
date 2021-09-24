using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class Test3Command : ChatCommand
    {
        public Test3Command() : base(3)
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

            ushort[] testa = new ushort[0];
            ushort[] testb = new ushort[0];

            //player.RoomPlayer.RoomSessionPlayer.WeaponManager.Replace(Game.Instance.WeaponManager.GetRandomWeapon());
            //player.RoomPlayer.RoomSessionPlayer.Post(new GCGameState(player.RoomPlayer.RoomSessionPlayer, 15)); // leave game
            //player.RoomPlayer.RoomSessionPlayer.Post(new GCStart(player.RoomPlayer.Room, player.PlayerId)); // start room
            //player.RoomPlayer.RoomSessionPlayer.Post(new GCJoin(player.RoomPlayer.RoomSessionPlayer)); // join game
            //player.RoomPlayer.RoomSessionPlayer.Post(new GCGameState(player.RoomPlayer.RoomSessionPlayer, 12)); // game state

            player.RoomPlayer.RoomSessionPlayer.Post(new GCPvEMoveNodesNpc(test1, testa, testb)); // weapon
        }
    }
}
