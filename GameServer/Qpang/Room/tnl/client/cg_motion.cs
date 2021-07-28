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
    public class CGMotion : GameNetEvent
    {
        private static NetClassRepInstance<CGMotion> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGMotion", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Cmd;
        public uint unk02;
        public uint unk03;
        public uint unk04;
        public uint unk05;
        public uint unk06;
        public uint unk07;
        public uint unk08;
        public uint unk09;
        public uint PlayerId;

        public CGMotion() : base(GameNetId.CG_MOTION, GuaranteeType.Unguaranteed, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }

        public override void Handle(GameConnection conn, Player player)
        {
            var roomPlayer = player.RoomPlayer;
            if (roomPlayer == null)
                return;

            var roomSession = roomPlayer.RoomSessionPlayer;
            if (roomSession == null)
                return;

            //roomSession.RelayPlaying<GCMotion>(Cmd, unk02, unk03, unk04, unk05, unk06, unk07, unk08, unk09, PlayerId);
        }
    }
}
