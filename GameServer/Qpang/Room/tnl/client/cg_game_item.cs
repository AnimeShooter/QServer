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
    public class CGGameItem : GameNetEvent
    {
        private static NetClassRepInstance<CGGameItem> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGGameItem", NetClassMask.NetClassGroupGameMask, 0);
        }

        public enum Commands : byte
        {
            PICKUP = 1,
            SPAWN = 6,
            REFILL_AMMO = 14,
        }

        public uint PlayerId;
        public uint unk02;
        public uint unk03;
        public uint unk04;
        public uint unk05;
        public uint unk06;
        public uint Uid;

        public CGGameItem() : base(GameNetId.CG_GAME_ITEM, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out unk02);
            bitStream.Read(out unk03);
            bitStream.Read(out unk04);
            bitStream.Read(out unk05);
            bitStream.Read(out unk06);
            bitStream.Read(out Uid);
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
            
            var roomSessionPlayer = roomPlayer.RoomSessionPlayer;
            if (roomSessionPlayer == null)
                return;

            if (roomSessionPlayer.Death)
                return;

            roomSessionPlayer.RoomSession.ItemManager.OnPickUp(roomSessionPlayer, Uid);
        }

    }
}
