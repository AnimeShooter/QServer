using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNL.Entities;
using TNL.Utils;
using TNL.Data;
using TNL.Types;

namespace Qserver.GameServer.Qpang
{
    public class CGPlayerChange : GameNetEvent
    {
        private static NetClassRepInstance<CGPlayerChange> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGPlayerChange", NetClassMask.NetClassGroupGameMask, 0);
        }

        public enum Commands : byte
        {
            CHARACTER = 1,
            TEAM = 2
        }

        public uint PlayerId;
        public byte Cmd;
        public uint Value;

        public CGPlayerChange() : base(GameNetId.CG_PLAYER_CHANGE, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out Cmd);
            bitStream.Read(out Value);
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }
        public override void Handle(GameConnection conn, Player player)
        {
            if(Cmd == (byte)Commands.CHARACTER)
            {
                if (Value != 333 && Value != 343 && Value != 578 && Value != 579 && Value != 850 && Value != 851)
                    return;

                player.Character = (ushort)Value;

                var roomPlayer = player.RoomPlayer;
                if (roomPlayer != null)
                    roomPlayer.Room.BroadcastWaiting<GCPlayerChange>(player, Cmd, Value);
            }
            else if(Cmd == (byte)Commands.TEAM)
            {
                var roomPlayer = player.RoomPlayer;
                if (roomPlayer == null)
                    return;

                if (roomPlayer.Playing)
                    return;

                var room = roomPlayer.Room;
                if (room.IsTeamAvailable((byte)Value))
                    roomPlayer.SetTeam((byte)Value);
            }
        }
    }
}
