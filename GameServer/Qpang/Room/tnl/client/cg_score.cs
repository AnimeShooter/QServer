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
    public class CGScore : GameNetEvent
    {
        private static NetClassRepInstance<CGScore> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGScore", NetClassMask.NetClassGroupGameMask, 0);
        }

        public enum Commands : uint
        {
            USER = 100,
            GAME = 200,
        }

        public uint Cmd;

        public CGScore() : base(GameNetId.CG_SCORE, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
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

            var session = roomPlayer.RoomSessionPlayer;
            if (session == null || session.RoomSession.Finished)
                return;

            if(Cmd == (uint)Commands.USER)
            {
                var roomSession = session.RoomSession;
                if (roomSession.Finished)
                    return;

                var players = roomSession.GetPlayingPlayers();
                // TODO: sort players?
                if(players != null) // ??
                    session.Post(new GCScore(players, session.RoomSession, 1));
            }
            else if(Cmd == (uint)Commands.GAME)
            {
                session.Post(new GCScore(session.RoomSession, (byte)(0xC8)));
            }

            // send twice cuz bugfix spectator psoition reset?
            session.Post(new GCGameState(player.PlayerId, 11, session.RoomSession.GetElapsedTime()));
            session.Post(new GCGameState(player.PlayerId, 11, session.RoomSession.GetElapsedTime()));
        }
    }
}
