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

        public uint PlayerId; // 140 SrcId
        public uint NpcUid; // 144 DstUd
        public uint Tick; // 88
        public float ImpactPosX; // 116 x
        public float ImpactPosY; // 120 y
        public float ImpactPosZ; // 124 z
        public float ImpactOffsetPosX; // 128 x
        public float ImpactOffsetPosY; // 132 y
        public float ImpactOffsetPosZ; // 136 z
        public uint EntityId; // 92 
        public byte HitType; // 148
        public byte BodyPartId; // 149 aka BodyPart
        public uint WeaponItemId; // 96
        public ulong RTT; // 104
        public byte WeaponType; // 112
        public byte HitLocation; // 150
        public byte KillCombo; // 151
        public uint Unk18; // 152
        public ushort DamageDealt; // 156 // extra unk1
        public byte KillCombo2; // 158 // extra unk2
        public uint RemainingHealth; // 160 // extra unk3


        public GCPvEHitNpc() : base(GameNetId.GC_PVE_HIT_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public GCPvEHitNpc(uint unk1, uint unk2, uint unk3, float srcx, float srcy, float srcz, float dstx, float dsty,
            float dstz, uint unk10, byte unk11, byte unk12, uint unk13, ulong unk14, byte unk15, byte unk16, byte unk17,
            uint unk18, ushort unk19, byte unk20, uint unk21) : base(GameNetId.GC_PVE_HIT_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            PlayerId = unk1; // srcId
            NpcUid = unk2; // dstrId
            Tick = unk3; // unk3
            ImpactPosX = srcx; // src x
            ImpactPosY = srcy; // src y
            ImpactPosZ = srcz; // src z
            ImpactOffsetPosX = dstx; // dst x
            ImpactOffsetPosY = dsty; // dst y
            ImpactOffsetPosZ = dstz; // dstz
            EntityId = unk10; // entityid
            HitType = unk11; // hitTyoe
            BodyPartId = unk12; // hitLocation
            WeaponItemId = unk13; // wepId
            RTT = unk14; // rtt
            WeaponType = unk15; // weaponType
            HitLocation = unk16;
            KillCombo = unk17;
            Unk18 = unk18;
            DamageDealt = unk19;
            KillCombo2 = unk20;
            RemainingHealth = unk21;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(PlayerId);
            bitStream.Write(NpcUid);
            bitStream.Write(Tick);
            bitStream.Write(ImpactPosX);
            bitStream.Write(ImpactPosY);
            bitStream.Write(ImpactPosZ);
            bitStream.Write(ImpactOffsetPosX);
            bitStream.Write(ImpactOffsetPosY);
            bitStream.Write(ImpactOffsetPosZ);
            bitStream.Write(EntityId);
            bitStream.Write(HitType);
            bitStream.Write(BodyPartId);
            bitStream.Write(WeaponItemId);
            bitStream.Write(RTT);
            bitStream.Write(WeaponType);
            bitStream.Write(HitLocation);
            bitStream.Write(KillCombo);
            bitStream.Write(Unk18);
            bitStream.Write(DamageDealt);
            bitStream.Write(KillCombo2);
            bitStream.Write(RemainingHealth);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out NpcUid);
            bitStream.Read(out Tick);
            bitStream.Read(out ImpactPosX);
            bitStream.Read(out ImpactPosY);
            bitStream.Read(out ImpactPosZ);
            bitStream.Read(out ImpactOffsetPosX);
            bitStream.Read(out ImpactOffsetPosY);
            bitStream.Read(out ImpactOffsetPosZ);
            bitStream.Read(out EntityId);
            bitStream.Read(out HitType);
            bitStream.Read(out BodyPartId);
            bitStream.Read(out WeaponItemId);
            bitStream.Read(out RTT);
            bitStream.Read(out WeaponType);
            bitStream.Read(out HitLocation);
            bitStream.Read(out KillCombo);
            bitStream.Read(out Unk18);
            bitStream.Read(out DamageDealt);
            bitStream.Read(out KillCombo2);
            bitStream.Read(out RemainingHealth);
        }
        public override void Process(EventConnection ps) { }
    }
}
