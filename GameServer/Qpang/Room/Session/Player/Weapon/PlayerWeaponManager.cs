using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class PlayerWeaponManager
    {
        private Weapon[] _weapons;
        private Weapon? _oldGun;
        private byte _selectedWeaponIndex;
        private Dictionary<uint, ushort[]> _defaultAmmo;
        private RoomSessionPlayer _player;

        private DateTime _lastShot;
        private uint _shotsFired;
        private uint _illegalShotsFired;

        public bool CanReload
        {
            get { return SelectedWeapon.ClipCount > 0; }
        }

        public bool CanShoot
        {
            get 
            { 
                if (HoldsMelee) 
                    return true; // idk

                if (SelectedWeapon.ClipSize == 0)
                    return false;

                ushort delay = 500;
                switch (this._selectedWeaponIndex)
                {
                    case 0: // throw?
                        delay = 500;
                        break;
                    case 1: // snipe
                        delay = 900; // 1400ms client delay?
                        break;
                    case 2: // gun
                        delay = this._player.RoomSession.PublicEnemy == this._player ? (ushort)102 : (ushort)45;
                        break;
                    case 3: // melee
                        delay = 400;
                        break;
                    default:
                        delay = 100; // idk
                        break;
                }

                if (this._lastShot.AddMilliseconds(delay) > DateTime.UtcNow)
                {
                    this._illegalShotsFired++;
                    if (this._shotsFired > 13 && this._illegalShotsFired / (float)this._shotsFired > 0.15f)
                    {
                        var allPlayers = Game.Instance.PlayersList();
                        foreach (var p in allPlayers)
                            if (p.Online)
                                p.Broadcast($"Player {this._player.Player.Name} has been removed for cheating!"); // public shaming!
                        this._player.RoomSession.RemovePlayer(this._player.Player.PlayerId); // be gone!
                    }
                    return false;
                }
                this._shotsFired++;
                this._lastShot = DateTime.UtcNow; // NOTE: Fuck you, Jarrett

                return true;
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

        public PlayerWeaponManager()
        {
            this._selectedWeaponIndex = 0; 
        }

        public void Initialize(RoomSessionPlayer player)
        {
            this._player = player;
            this._weapons = new Weapon[4];
            this._defaultAmmo = new Dictionary<uint, ushort[]>();

            var equipmentManager = this._player.Player.EquipmentManager;
            var itemIds = equipmentManager.GetWeaponItemIdsByCharacter(this._player.Character);
            var hasPreSelectedWeapons = false;
            var isMeleeOnly = player.RoomSession.Room.MeleeOnly;

            for (int i = 0; i < itemIds.Length; i++)
            {
                var weapon = Game.Instance.WeaponManager.Get(itemIds[i]);

                if (isMeleeOnly)
                {
                    if (i == 3)
                        this._weapons[3] = weapon;
                }
                else
                    this._weapons[i] = weapon;

                if(!this._defaultAmmo.ContainsKey(weapon.ItemId))
                    this._defaultAmmo.Add(weapon.ItemId, new ushort[2]
                    {
                        (ushort)(weapon.ClipCount + equipmentManager.GetExtraAmmoForWeaponIndex((byte)i)),
                        (ushort)(weapon.ClipSize)
                    });

                if (this._weapons[i].ItemId != 0 && !hasPreSelectedWeapons)
                {
                    this._selectedWeaponIndex = (byte)i;
                    hasPreSelectedWeapons = true;
                }
            }
        }

        public void InitPrey()
        {
            //this._weapons[0] = 0; // throw
            //this._weapons[1] = 0; // sniper
            this._selectedWeaponIndex = 2; // gun
            //this._weapons[3] = 0; // melee

            // save last gun
            if (!this._oldGun.HasValue)
                this._oldGun = this._weapons[this._selectedWeaponIndex];

            Weapon weapon = new Weapon()
            {
                ItemId = 1095368720,
                ClipCount = 2,
                ClipSize = 50
            };

            if (!this._defaultAmmo.ContainsKey(weapon.ItemId))
                this._defaultAmmo.Add(weapon.ItemId, new ushort[2]
                {
                        (ushort)(weapon.ClipCount),
                        (ushort)(weapon.ClipSize)
                });

            this._weapons[this._selectedWeaponIndex] = weapon;
        }

        public void Reset()
        {
            if (this._player == null)
                return;

            if(this._oldGun.HasValue)
                this._weapons[2] = this._oldGun.Value;

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

        public void Reload(ulong seqId = 0)
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
            // NOTE: allow reload glitch
            //if (SelectedWeapon.ItemId == weaponId)
            //    return; // already picked
 
            for (int i = 0; i < this._weapons.Length; i++)
            {
                if(this._weapons[i].ItemId == weaponId)
                {
                    this._selectedWeaponIndex = (byte)i;
                    break;
                }   
            }

            if (SelectedWeapon.ItemId == weaponId && this._selectedWeaponIndex == 1) // 1 == sniper?
                this._player.OnReswap();

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
