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
    public class CGStart : GameNetEvent
    {
        private static NetClassRepInstance<CGStart> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGStart", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public byte unk01;

        public CGStart() : base(GameNetId.CG_START, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out unk01);
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

            if (player.PlayerId != roomPlayer.Room.MasterId)
                return;

            roomPlayer.Room.Start(player);
        }
    }
}
