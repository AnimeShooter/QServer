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
    public class CGPvEHitNpc : GameNetEvent
    {
        private static NetClassRepInstance<CGPvEHitNpc> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGPvEHitNpc", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint SrcId;
        public uint DstId;
        public uint Unk03;
        public float SrcX;
        public float SrcY;
        public float SrcZ;
        public float DstX;
        public float DstY;
        public float DstZ;
        public uint Unk10;
        public byte HitType;
        public byte HitLocation;
        public uint WeaponId;
        public ulong RTT;
        public byte Unk15;
        public uint Unk16;
        public byte Unk17;
        public byte Unk18;
        public uint Unk19;

        public CGPvEHitNpc() : base(GameNetId.CG_PVE_HIT_NPC, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out SrcId);
            bitStream.Read(out DstId);
            bitStream.Read(out Unk03);
            bitStream.Read(out SrcX);
            bitStream.Read(out SrcY);
            bitStream.Read(out SrcZ);
            bitStream.Read(out DstX);
            bitStream.Read(out DstY);
            bitStream.Read(out DstZ);
            bitStream.Read(out Unk10);
            bitStream.Read(out HitType);
            bitStream.Read(out HitLocation);
            bitStream.Read(out WeaponId);
            bitStream.Read(out RTT);
            bitStream.Read(out Unk15);
            bitStream.Read(out Unk16);
            bitStream.Read(out Unk17);
            bitStream.Read(out Unk18);
            bitStream.Read(out Unk19);
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
            if (session.Death) // || !weaponManager.CanShoot)
                return;

            // TODO handle

            session.RoomSession.RelayPlaying<GCPvEHitNpc>(SrcId, DstId, Unk03, SrcX, SrcY, SrcZ, DstX, DstY, DstZ,
                Unk10, HitType, HitLocation, WeaponId, RTT, Unk15, (byte)0, (byte)99, (uint)Unk18, (ushort)Unk19, (byte)0, (uint)0);
        }
    }
}
