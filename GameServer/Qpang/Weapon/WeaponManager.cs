using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Qpang;
using Qserver.Util;

namespace Qserver.GameServer.Qpang
{
    public class WeaponManager
    {
        private Dictionary<uint, Weapon> _weapons;
        private Dictionary<ushort, byte> _characterPower = new Dictionary<ushort, byte>()
        {
            {333, 4},
            {343, 3},
            {578, 8},
            {579, 2},
            {850, 5},
            {851, 4}
        };

        private object _lock;

        public WeaponManager()
        {
            this._lock = new object();
            this._weapons = new Dictionary<uint, Weapon>();

            var weapons = Game.Instance.ItemsRepository.GetWeapons().Result;
            foreach(var w in weapons)
            {
                this._weapons.Add(w.item_id, new Weapon()
                {
                    ItemId = w.item_id,
                    Damage = w.damage,
                    ClipSize = w.clip_size,
                    ClipCount = w.clip_amount,
                    Weight = w.weight,
                    EffectId = w.effect_id,
                    EffectChance = w.chance,
                    EffectDuration = w.duration,
                    WeaponType = GetWeaponType(w.item_id)
                });
            }
			Log.Message(LogType.MISC, $"WeaponManager loaded {this._weapons.Count} Weapons from the database!");
        }

		public bool CanEquip(uint weaponId, ushort characterId)
        {
			var weapon = Get(weaponId);
			if (!this._characterPower.ContainsKey(characterId))
				return false;

			return weapon.Weight <= this._characterPower[characterId];
		}

		public Weapon Get(uint weaponId)
        {
			lock(this._lock)
            {
				if (this._weapons.ContainsKey(weaponId))
					return this._weapons[weaponId];
				return null;
            }
        }

        public WeaponType GetWeaponType(uint id)
        {
			switch (id)
			{
				case 1095172098:
				case 1095172100:
				case 1095172102:
				case 1095172103:
				case 1095172104:
				case 1095172105:
				case 1095172130:
				case 1095172131:
				case 1095761958:
				case 1095172147:
				case 1095172160:
				case 1095172097:
					return WeaponType.MELEE;
				case 1095237632:
				case 1095303169:
				case 1095761921:
				case 1095303170:
				case 1095303171:
				case 1095303172:
				case 1095303173:
				case 1095303177:
				case 1095303179:
				case 1095303180:
				case 1095303181:
				case 1095368720:
				case 1095303185:
				case 1095237671:
				case 1095237672:
				case 1095761961:
				case 1095303231:
				case 1095303233:
					return WeaponType.RIFLE;
				case 1095368704:
				case 1095368706:
				case 1095368707:
				case 1095368708:
				case 1095368709:
				case 1095368710:
				case 1095368711:
				case 1095368713:
				case 1095368716:
				case 1095368717:
				case 1095434253:
				case 1095368718:
				case 1095368719:
				case 1095368721:
				case 1095368734:
				case 1095368746:
				case 1095368756:
				case 1095368757:
				case 1095368758:
				case 1095368764:
				case 1095368766:
					return WeaponType.LAUNCHER;
				case 1095434240:
				case 1095499776:
				case 1095434241:
				case 1095499777:
				case 1095434242:
				case 1095499778:
				case 1095434243:
				case 1095499779:
				case 1095434244:
				case 1095434246:
				case 1095434249:
				case 1095434252:
				case 1095434255:
				case 1095434256:
				case 1095434257:
				case 1095434258:
				case 1095434277:
				case 1095434285:
				case 1095434287:
				case 1095434290:
				case 1095434295:
				case 1095434296:
				case 1095434297:
				case 1095434299:
				case 1095499837:
				default:
					return WeaponType.BOMB;
			}
		}
    }
}
