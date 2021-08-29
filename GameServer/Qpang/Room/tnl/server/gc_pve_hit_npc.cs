using TNL.Data;
using TNL.Entities;
using TNL.Types;
using TNL.Utils;

namespace Qserver.GameServer.Qpang
{
    public class GCPvEHitNpc : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEHitNpc> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEHitNpc", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint SrcId; // 140
        public uint DstId; // 144
        public uint Tick; // 88
        public float SrcX; // 116 x
        public float SrcY; // 120 y
        public float SrcZ; // 124 z
        public float DstX; // 128 x
        public float DstY; // 132 y
        public float DstZ; // 136 z
        public uint Unk10; // 92 
        public byte HitType; // 148
        public byte HitLocation; // 149
        public uint Unk13; // 96
        public ulong RTT; // 104
        public byte WeaponType; // 112
        public byte Unk16; // 150
        public byte KillCombo; // 151
        public uint Unk18; // 152
        public ushort Unk19; // 156 // extra unk1
        public byte Unk20; // 158 // extra unk2
        public uint Unk21; // 160 // extra unk3


        public GCPvEHitNpc() : base(GameNetId.GC_PVE_HIT_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public GCPvEHitNpc(uint unk1, uint unk2, uint unk3, float srcx, float srcy, float srcz, float dstx, float dsty,
            float dstz, uint unk10, byte unk11, byte unk12, uint unk13, ulong unk14, byte unk15, byte unk16, byte unk17,
            uint unk18, ushort unk19, byte unk20, uint unk21) : base(GameNetId.GC_PVE_HIT_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            SrcId = unk1; // srcId
            DstId = unk2; // dstrId
            Tick = unk3; // unk3
            SrcX = srcx; // src x
            SrcY = srcy; // src y
            SrcZ = srcz; // src z
            DstX = dstx; // dst x
            DstY = dsty; // dst y
            DstZ = dstz; // dstz
            Unk10 = unk10; // entityid
            HitType = unk11; // hitTyoe
            HitLocation = unk12; // hitLocation
            Unk13 = unk13; // wepId
            RTT = unk14; // rtt
            WeaponType = unk15; // weaponType
            Unk16 = unk16;
            KillCombo = unk17;
            Unk18 = unk18;
            Unk19 = unk19;
            Unk20 = unk20;
            Unk21 = unk21;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(SrcId);
            bitStream.Write(DstId);
            bitStream.Write(Tick);
            bitStream.Write(SrcX);
            bitStream.Write(SrcY);
            bitStream.Write(SrcZ);
            bitStream.Write(DstX);
            bitStream.Write(DstY);
            bitStream.Write(DstZ);
            bitStream.Write(Unk10);
            bitStream.Write(HitType);
            bitStream.Write(HitLocation);
            bitStream.Write(Unk13);
            bitStream.Write(RTT);
            bitStream.Write(WeaponType);
            bitStream.Write(Unk16);
            bitStream.Write(KillCombo);
            bitStream.Write(Unk18);
            bitStream.Write(Unk19);
            bitStream.Write(Unk20);
            bitStream.Write(Unk21);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out SrcId);
            bitStream.Read(out DstId);
            bitStream.Read(out Tick);
            bitStream.Read(out SrcX);
            bitStream.Read(out SrcY);
            bitStream.Read(out SrcZ);
            bitStream.Read(out DstX);
            bitStream.Read(out DstY);
            bitStream.Read(out DstZ);
            bitStream.Read(out Unk10);
            bitStream.Read(out HitType);
            bitStream.Read(out HitLocation);
            bitStream.Read(out Unk13);
            bitStream.Read(out RTT);
            bitStream.Read(out WeaponType);
            bitStream.Read(out Unk16);
            bitStream.Read(out KillCombo);
            bitStream.Read(out Unk18);
            bitStream.Read(out Unk19);
            bitStream.Read(out Unk20);
            bitStream.Read(out Unk21);
        }
        public override void Process(EventConnection ps) { }
    }
}
