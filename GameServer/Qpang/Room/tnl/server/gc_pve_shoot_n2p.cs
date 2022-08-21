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
    public class GCPvEShootN2P : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEShootN2P> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEShootN2P", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint NpcUid; // 88
        public uint WeaponBodyPartId; // 92
        public uint TargetX; // 96 // idk?
        public uint TargetY; // 100
        public uint TargetZ; // 104
        public long unk6; // 112 wepid?

        public GCPvEShootN2P() : base(GameNetId.GC_PVE_SHOOT_N2P, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(NpcUid);
            bitStream.Write(WeaponBodyPartId);
            bitStream.Write(TargetX);
            bitStream.Write(TargetY);
            bitStream.Write(TargetZ);
            bitStream.Write(unk6);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out NpcUid);
            bitStream.Read(out WeaponBodyPartId);
            bitStream.Read(out TargetX);
            bitStream.Read(out TargetY);
            bitStream.Read(out TargetZ);
            bitStream.Read(out unk6);
        }
        public override void Process(EventConnection ps) { }
    }
}
