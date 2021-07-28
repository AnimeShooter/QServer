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
    public class CGMove : GameNetEvent
    {
        private static NetClassRepInstance<CGMove> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGMove", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint Cmd;
        public float PosX;
        public float PosY;
        public float PosZ;
        public float unk04;
        public float unk05;
        public float unk06;
        public float unk07;
        public float unk08;
        public float unk09;
        public float unk10;
        //public uint unk11;

        public CGMove() : base(GameNetId.CG_MOVE, GuaranteeType.Unguaranteed, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out Cmd);
            bitStream.Read(out PosX);
            bitStream.Read(out PosY);
            bitStream.Read(out PosZ);
            bitStream.Read(out unk04);
            bitStream.Read(out unk05);
            bitStream.Read(out unk06);
            bitStream.Read(out unk07);
            bitStream.Read(out unk08);
            bitStream.Read(out unk09);
            bitStream.Read(out unk10);
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
            //if (roomSession != null)
            //    roomSession.RelayPlayingExcept<GCMove>(player.PlayerId, PlayerId, Cmd, PosX, PosY, PosZ, unk04, unk05, unk06, unk07, unk08, unk09, unk10);
        }
    }
}
