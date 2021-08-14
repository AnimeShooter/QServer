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

    public struct Weapon
    {
        public uint ItemId;
        public ushort Damage;
        public ushort ClipSize;
        public byte ClipCount;
        public byte Weight; // = 99;
        public byte EffectId;
        public byte EffectChance;
        public byte EffectDuration; // seconds
        public WeaponType WeaponType; // WeaponType.MELEE;

        //public Weapon()
        //{
        //    Weight = 99;
        //    WeaponType = WeaponType.MELEE;
        //}
    }
}
