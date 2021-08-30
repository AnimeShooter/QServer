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
    public class CGPvEGetItem : GameNetEvent
    {
        private static NetClassRepInstance<CGPvEGetItem> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGPvEGetItem", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint Uid;

        public CGPvEGetItem() : base(GameNetId.CG_PVE_GET_ITEM, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out Uid);
        }
        public override void Process(EventConnection ps) 
        {
            Post(ps);
        }

        public override void Handle(GameConnection conn, Player player)
        {
            if (player == null || player.RoomPlayer == null || player.RoomPlayer.RoomSessionPlayer == null)
                return;

            player.RoomPlayer.RoomSessionPlayer.RoomSession.PvEEntityManager.ItemPickup(player.RoomPlayer.RoomSessionPlayer, Uid);
        }
    }
}
