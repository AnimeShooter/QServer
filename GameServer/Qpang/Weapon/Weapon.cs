using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public enum WeaponType : byte
    {
        MELEE,
        RIFLE,
        LAUNCHER,
        BOMB
    }

    public class Weapon
    {
        public uint ItemId;
        public ushort Damage;
        public ushort ClipSize;
        public byte ClipCount;
        public byte Weight = 99;
        public byte EffectId;
        public byte EffectChance;
        public byte EffectDuration; // seconds
        public WeaponType WeaponType = WeaponType.MELEE;
    }
}
