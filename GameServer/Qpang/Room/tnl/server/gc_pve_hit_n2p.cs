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
    public class GCPvEHitN2P : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEHitN2P> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEHitN2P", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint NpcUid; // 88
        public uint NpcBodyPartId; // 92
        public float ImpactPosX; // 96
        public float ImpactPosY; // 100
        public float ImpactPosZ; // 104
        public float ImpactOffsetX; // 108
        public float ImpactOffsetY; // 112
        public float ImpactOffsetZ; // 116
        public byte Unk9; // 120
        public byte BodyPartHit; // 121
        public uint PlayerId; // 124
        public uint RemainingHealth; // 128

        public GCPvEHitN2P() : base(GameNetId.GC_PVE_HIT_N2P, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(NpcUid);
            bitStream.Write(NpcBodyPartId);
            bitStream.Write(ImpactPosX);
            bitStream.Write(ImpactPosY);
            bitStream.Write(ImpactPosZ);
            bitStream.Write(ImpactOffsetX);
            bitStream.Write(ImpactOffsetY);
            bitStream.Write(ImpactOffsetZ);
            bitStream.Write(Unk9);
            bitStream.Write(BodyPartHit);
            bitStream.Write(PlayerId);
            bitStream.Write(RemainingHealth);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out NpcUid);
            bitStream.Read(out NpcBodyPartId);
            bitStream.Read(out ImpactPosX);
            bitStream.Read(out ImpactPosY);
            bitStream.Read(out ImpactPosZ);
            bitStream.Read(out ImpactOffsetX);
            bitStream.Read(out ImpactOffsetY);
            bitStream.Read(out ImpactOffsetZ);
            bitStream.Read(out Unk9);
            bitStream.Read(out BodyPartHit);
            bitStream.Read(out PlayerId);
            bitStream.Read(out RemainingHealth);
        }
        public override void Process(EventConnection ps)
        {

        }
    }
}
