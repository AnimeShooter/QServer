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
    public class CGShoot : GameNetEvent
    {
        private static NetClassRepInstance<CGShoot> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGShoot", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint unk02;
        public uint unk03;
        public float SrcX;
        public float SrcY;
        public float SrcZ;
        public float DstX;
        public float DstY;
        public float DstZ;
        public uint EntityId;
        public uint ItemId;
        public ulong CardId;
        public ushort Cmd;
        public uint unk11;

        public CGShoot() : base(GameNetId.CG_SHOOT, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out unk02);
            bitStream.Read(out unk03);
            bitStream.Read(out SrcX);
            bitStream.Read(out SrcY);
            bitStream.Read(out SrcZ);
            bitStream.Read(out DstX);
            bitStream.Read(out DstY);
            bitStream.Read(out DstY);
            bitStream.Read(out EntityId);
            bitStream.Read(out ItemId);
            bitStream.Read(out CardId);
            bitStream.Read(out Cmd);
            bitStream.Read(out unk11);
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }

        public override void Handle(GameConnection conn, Player player)
        {
            var roomPlayer = player.RoomPlayer;
            if (roomPlayer == null || roomPlayer.Spectating)
                return;

            var session = roomPlayer.RoomSessionPlayer;
            if (session == null)
                return;

            var weaponManager = session.WeaponManager;
            if (session.Death) // || !weaponManager.CanShoot())
                return;

            if (session.Invincible)
                session.RemoveInvincibility();

            var playerId = player.PlayerId;
            //weaponManager.Shoot(EntityId); // TODO

            if (ItemId != 1095434246) //  Octo NOTE: add alll mines
                session.EntityManager.Shoot(EntityId);

            // TODO
            session.RoomSession.RelayPlayingExcept<GCShoot>(playerId, playerId, ItemId, SrcX, SrcY, SrcZ, DstX, DstY, DstZ, EntityId, (uint)0);
        }

    }
}
