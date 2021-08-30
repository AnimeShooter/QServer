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
    public class CGPvEEventObject : GameNetEvent
    {
        private static NetClassRepInstance<CGPvEEventObject> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGPvEEventObject", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Uid;
        public bool Triggered;

        public CGPvEEventObject() : base(GameNetId.CG_PVE_EVENT_OBJECT, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Uid);
            bitStream.Read(out Triggered);
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }

        public override void Handle(GameConnection conn, Player player)
        {
            if (player == null || player.RoomPlayer == null || player.RoomPlayer.RoomSessionPlayer == null)
                return;

            var roomPlayer = player.RoomPlayer;
            if (roomPlayer == null)
                return;

            var roomSessionPlayer = roomPlayer.RoomSessionPlayer;
            if (roomSessionPlayer == null)
                return;

            if (roomSessionPlayer.Death)
                return;

            roomSessionPlayer.RoomSession.PvEEntityManager.InvokeObject(roomSessionPlayer, Uid, Triggered);
        }
    }
}
