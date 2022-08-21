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
    public class GCPvEHitObject : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEHitObject> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEHitObject", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId; // 88
        public uint ObjectUid; // 92
        public uint WeaponItemId; // 96
        public byte Cmd; // 100
        public ushort Unk5; // 102 poss despawn
        public ushort DamageDealt; // 104

        public GCPvEHitObject() : base(GameNetId.GC_PVE_HIT_OBJECT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(PlayerId);
            bitStream.Write(ObjectUid);
            bitStream.Write(WeaponItemId);
            bitStream.Write(Cmd);
            bitStream.Write(Unk5);
            bitStream.Write(DamageDealt);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out ObjectUid);
            bitStream.Read(out WeaponItemId);
            bitStream.Read(out Cmd);
            bitStream.Read(out Unk5);
            bitStream.Read(out DamageDealt);
        }
        public override void Process(EventConnection ps)
        {

        }
    }
}
