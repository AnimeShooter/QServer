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
    public class CGPvEStart : GameNetEvent
    {
        private static NetClassRepInstance<CGPvEStart> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGPvEStart", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint MasterUid;
        public byte Unk02;

        public CGPvEStart() : base(GameNetId.CG_PVE_START, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out MasterUid);
            bitStream.Read(out Unk02);
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

            if (roomPlayer.Room.Playing)
                return;

            if (roomPlayer.Player.PlayerId != roomPlayer.Room.MasterId)
                return;

            roomPlayer.Room.Start();

            // 122: stage2 simple
        }
    }
}
