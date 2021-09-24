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
    public class CGPvERespawnReq : GameNetEvent
    {
        private static NetClassRepInstance<CGPvERespawnReq> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGPvERespawnReq", NetClassMask.NetClassGroupGameMask, 0);
        }

        public bool Accepted; // 88

        public CGPvERespawnReq() : base(GameNetId.CG_PVE_RESPAWN_REQ, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out Accepted);
        }
        public override void Process(EventConnection ps) 
        {
            Post(ps);
        }

        public override void Handle(GameConnection conn, Player player)
        {
            if (player == null || player.RoomPlayer == null || player.RoomPlayer.RoomSessionPlayer == null)
                return;

            // TODO: ask for a respawn?
            player.RoomPlayer.RoomSessionPlayer.Respawn();

            // player.RoomPlayer.Room.RoomSession.CompletePvERound();
        }
    }
}
