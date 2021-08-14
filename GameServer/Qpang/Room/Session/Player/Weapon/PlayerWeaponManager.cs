using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class PlayerWeaponManager
    {
        private Weapon[] _weapons;
        private byte _selectedWeaponIndex;
        private Dictionary<uint, ushort[]> _defaultAmmo;
        private RoomSessionPlayer _player;

        public bool CanReload
        {
            get { return SelectedWeapon.ClipCount > 0; }
        }

        public bool CanShoot
        {
            get 
            { 
                if(HoldsMelee) return false;
                return SelectedWeapon.ClipSize > 0;
            }
        }

        public bool HoldsMelee
        {
            get { return this._selectedWeaponIndex == 3; }
        }

        public Weapon[] Weapons
        {
            get { return this._weapons; }
        }

        public Weapon SelectedWeapon
        {
            get { return this._weapons[this._selectedWeaponIndex]; }
        }

        public uint[] WeaponIds
        {
            get 
            {
                uint[] wepIds = new uint[4];
                for (int i = 0; i < wepIds.Length; i++)
                    wepIds[i] = this._weapons[i].ItemId;

                return wepIds;
            }
        }

        public PlayerWeaponManager(RoomSessionPlayer player)
        {
            this._player = player;
        }

        public void Reset()
        {
            if (this._player == null)
                return;

            lock(this._player.Lock)
            {
                for(int i = 0; i < this._weapons.Length; i++)
                {
                    var ammo = this._defaultAmmo[this._weapons[i].ItemId];

                    this._weapons[i].ClipCount = (byte)ammo[0];
                    this._weapons[i].ClipSize = ammo[1];

                    // create ammo clip item xD
                    this._player.Post(new GCGameItem(14, new List<GCGameItem.Item>() { new GCGameItem.Item() { ItemId = 1191182337, ItemUid = 1 } }, this._weapons[i].ItemId));
                }
            }
        }

        public void Reload(uint seqId = 0)
        {
            this._weapons[this._selectedWeaponIndex].ClipCount--;
            this._weapons[this._selectedWeaponIndex].ClipSize = this._defaultAmmo[SelectedWeapon.ItemId][1];

            if (this._player == null)
                return;

            lock (this._player.Lock)
                this._player.RoomSession.RelayPlaying<GCWeapon>(this._player.Player.PlayerId, (uint)3, SelectedWeapon.ItemId, (ulong)seqId, (ushort)0, false, (ushort)75);
        }

        public void Shoot(uint entityId)
        {
            this._weapons[this._selectedWeaponIndex].ClipSize--;
        }

        public bool HasWeapon(uint weaponId)
        {
            foreach (var weapon in this._weapons)
                if (weapon.ItemId == weaponId)
                    return true;

            return true;
        }

        public void SwitchWeapon(uint weaponId)
        {
            if (SelectedWeapon.ItemId == weaponId)
                return; // already picked

            for(int i = 0; i < this._weapons.Length; i++)
            {
                if(this._weapons[i].ItemId == weaponId)
                {
                    this._selectedWeaponIndex = (byte)i;
                    break;
                }   
            }

            if (this._player == null)
                return;

            lock (this._player.Lock)
            {
                this._player.RoomSession.RelayPlaying<GCWeapon>(this._player.Player.PlayerId, (uint)0, SelectedWeapon.ItemId, (ulong)0, (ushort)0, false, (ushort)75);
                this._player.Post(new GCWeapon(this._player.Player.PlayerId, 5, SelectedWeapon.ItemId, 0));
            }
        }

        public void RefillCurrentWeapon()
        {
            var itemId = SelectedWeapon.ItemId;
            var ammo = this._defaultAmmo[itemId];

            this._weapons[this._selectedWeaponIndex].ClipCount = (byte)ammo[0];
            this._weapons[this._selectedWeaponIndex].ClipSize = ammo[1];

            if (this._player == null)
                return;

            lock (this._player.Lock)
                this._player.Post(new GCGameItem(14, new List<GCGameItem.Item>()
                {
                    new GCGameItem.Item()
                    {
                        ItemId = 1191182337,
                        ItemUid = 1,
                        SkillId = 0,
                        X = 0,
                        Y = 0,
                        Z = 0
                    }
                }, itemId));
        }
    }
}
