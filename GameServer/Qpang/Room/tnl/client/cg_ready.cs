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
    public class CGReady : GameNetEvent
    {
        private static NetClassRepInstance<CGReady> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGReady", NetClassMask.NetClassGroupGameMask, 0);
        }

        public enum Commands : uint
        {
            UNREADY = 0,
            READY = 1
        };

        public uint PlayerId;
        public uint Cmd;
        
        public CGReady() : base(GameNetId.CG_READY, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out Cmd);
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }

        public override void Handle(GameConnection conn, Player player)
        {
            var roomPlayer = player.RoomPlayer;
            if (roomPlayer == null)
                return;

            if (roomPlayer.Playing)
                return;

            if (Cmd == (uint)Commands.UNREADY)
                roomPlayer.SetReady(false);
            else if (Cmd == (uint)Commands.READY)
            {
                var room = roomPlayer.Room;
                if (room.Playing)
                {
                    if (!room.CanStartInTeam(roomPlayer.Team))
                    {
                        player.Broadcast("There are to many players in this team!");
                        return;
                    }

                    // reject from joining PvE
                    if(room.RoomSession.GameMode.IsPvE())
                    {
                        player.Broadcast("Please wait for the match to finish!");
                        return;
                    }

                    var session = room.RoomSession;
                    if (session != null && session.IsAlmostFinished())
                    {
                        player.Broadcast("Please wait, this match is almost over!");
                        return;
                    }

                    roomPlayer.SetReady(true);
                    roomPlayer.Playing = true;
                    conn.StartLoading(room, roomPlayer);
                    roomPlayer.OnStart();

                }
                else
                    roomPlayer.SetReady(true);
            }
            

        }
    }
}
