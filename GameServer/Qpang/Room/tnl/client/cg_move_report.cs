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
    public class CGMoveReport : GameNetEvent
    {
        private static NetClassRepInstance<CGMoveReport> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGMoveReport", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public float PosX;
        public float PosY;
        public float PosZ;
        public float unk05; // possible pitch
        public float unk06; // possible yaw (NOTE: used to anti cheat detection)


        public CGMoveReport() : base(GameNetId.CG_MOVE_REPORT, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out PosX);
            bitStream.Read(out PosY);
            bitStream.Read(out PosZ);
            bitStream.Read(out unk05);
            bitStream.Read(out unk06);
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

            if (roomPlayer.Spectating)
                return;

            var session = roomPlayer.RoomSessionPlayer;
            if (session == null)
                return;

            if (session.Death)
                return;

            session.Position = new Position() { X = PosX, Y = PosY, Z = PosZ };

            var roomSession = roomPlayer.Room.RoomSession;

            // TODO: fix timing issue and detect cheatingg
            if (roomSession != null)
            {
                // TODO
                //roomSession.RelayPlayingExcept<GCMove>(player.PlayerId, PlayerId, 0, PosX, PosY, PosZ, unk04, unk05, unk06, Pitch, Yawn, Tick, Unk10);
            }
        }
    }
}
