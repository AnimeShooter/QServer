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
    public class GCWeapon : GameNetEvent
    {
        private static NetClassRepInstance<GCWeapon> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCWeapon", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint Cmd = 100;
        public uint WeaponId;
        public ulong WeaponSeqId;
        public ushort WeaponOpt; // effects/types/etc
        public byte OutOfAmmo;
        public ushort MagazineId;

        public GCWeapon() : base(GameNetId.GC_WEAPON, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public GCWeapon(uint playerId, uint cmd, uint weaponId, ulong weaponSeqId, ushort weaponOpt = 0, bool outOfAmmo = false, ushort magazineId = 75) : base(GameNetId.GC_WEAPON, GuaranteeType.Guaranteed, EventDirection.DirServerToClient)
        {
            PlayerId = playerId;
            Cmd = cmd;
            WeaponId = weaponId;
            WeaponSeqId = weaponSeqId;
            WeaponOpt = weaponOpt;
            OutOfAmmo = outOfAmmo ? (byte)1 : (byte)0;
            MagazineId = magazineId;
        }

        public GCWeapon(uint playerId, uint cmd, ushort weaponOpt = 0) : base(GameNetId.GC_WEAPON, GuaranteeType.Guaranteed, EventDirection.DirServerToClient)
        {
            PlayerId = playerId;
            Cmd = cmd;
            WeaponOpt = weaponOpt;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(PlayerId);
            bitStream.Write(Cmd);
            bitStream.Write(WeaponId);
            bitStream.Write(WeaponSeqId);
            bitStream.Write(WeaponOpt);
            bitStream.Write(OutOfAmmo);
            bitStream.Write(MagazineId);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
