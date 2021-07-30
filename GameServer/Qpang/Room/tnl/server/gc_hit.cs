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
    public class GCHit : GameNetEvent
    {
        private static NetClassRepInstance<GCHit> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCHit", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint SrcPlayerId;
        public uint DstPlayerId;
        public uint Unk03;
        public float SrcPosX;
        public float SrcPosY;
        public float SrcPosZ;
        public float DstPosX;
        public float DstPosY;
        public float DstPosZ;
        public float Count;
        public byte HitType;
        public byte HitLocation;
        public ushort HealthLeft;
        public ushort DamageDealt;
        public uint WeaponId;
        public ulong RTT;
        public uint WeaponType;
        public byte Unk16;
        public byte KillCombo;
        public byte Unk18;
        public byte WeaponEffect;
        public byte Unk20;
        public byte Unk21;
        public uint Unk22;
        
        public GCHit() : base(GameNetId.GC_HIT, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }
        public GCHit(uint srcId, uint dstId, uint unk03, float srcPosX, float srcPosY, float srcPosZ, float dstPosX, float dstPosY, float dstPosZ, uint count, byte hitType, byte hitLocation, ushort healthLeft,
            ushort damage, uint weaponId, ulong rtt, uint weaponType, byte unk16, byte killCount, byte unk18, byte effect, byte unk20 = 0, byte unk21 = 0, byte unk22 = 0) : base(GameNetId.GC_HIT, GuaranteeType.Guaranteed, EventDirection.DirServerToClient) 
        {
            SrcPlayerId = srcId;
            DstPlayerId = dstId;
            Unk03 = unk03;
            SrcPosX = srcPosX;
            SrcPosY = srcPosY;
            SrcPosZ = srcPosZ;
            DstPosX = dstPosX;
            DstPosY = dstPosY;
            DstPosZ = dstPosZ;
            Count = count;
            HitType = hitType;
            HitLocation = hitLocation;
            HealthLeft = healthLeft;
            DamageDealt = damage;
            WeaponId = weaponId;
            RTT = rtt;
            WeaponType = weaponType;
            Unk16 = unk16;
            KillCombo = killCount;
            Unk18 = unk18;
            WeaponEffect = effect;
            Unk20 = unk20;
            Unk21 = unk21;
            Unk22 = unk22;
        }
        public GCHit(uint srcId, uint dstId, ushort damage, ushort healthLeft) : base(GameNetId.GC_HIT, GuaranteeType.Guaranteed, EventDirection.DirServerToClient)
        {
            SrcPlayerId = srcId;
            SrcPlayerId = dstId;
            DamageDealt = damage;
            HealthLeft = healthLeft;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(SrcPlayerId);
            bitStream.Write(DstPlayerId);
            bitStream.Write(Unk03);
            bitStream.Write(SrcPosX);
            bitStream.Write(SrcPosY);
            bitStream.Write(SrcPosZ);
            bitStream.Write(DstPosX);
            bitStream.Write(DstPosY);
            bitStream.Write(DstPosZ);
            bitStream.Write(Count);
            bitStream.Write(HitType);
            bitStream.Write(HitLocation);
            bitStream.Write(HealthLeft);
            bitStream.Write(DamageDealt);
            bitStream.Write(WeaponId);
            bitStream.Write(RTT);
            bitStream.Write(WeaponType);
            bitStream.Write(Unk16);
            bitStream.Write(KillCombo);
            bitStream.Write(Unk18);
            bitStream.Write(WeaponEffect);
            bitStream.Write(Unk20);
            bitStream.Write(Unk21);
            bitStream.Write(Unk22);
            bitStream.Write((byte)0); // is P2P
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
