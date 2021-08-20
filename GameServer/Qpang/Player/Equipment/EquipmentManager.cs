using Qserver.GameServer.Network.Managers;
using System.Collections.Generic;
using Qserver.GameServer.Network.Managers;

namespace Qserver.GameServer.Qpang
{
    public class EquipmentManager
    {
        public static ushort[] CharacterIds = new ushort[] { 333, 343, 578, 579, 850, 851 };

        private Dictionary<ushort, ulong[]> _equips; // long[13]
        private List<ushort> _unlockedCharacters;
        private ulong[] _skillCards; // long[3]
        private object _lock;
        private Player _player;
        private List<ulong> _functionCards;
        private object _functionCardlock;
        private object _skillCardlock;

        public List<ushort> UnlockerCharacters
        {
            get { return _unlockedCharacters; }
        }

        public EquipmentManager(Player player)
        {
            this._player = player;
            this._unlockedCharacters = new List<ushort>() { 333, 343, 578, 579, 850, 851 }; // Hardcode all unlocked
            this._functionCards = new List<ulong>();
            this._lock = new object();
            this._functionCardlock = new object();
            this._skillCardlock = new object();
            this._equips = new Dictionary<ushort, ulong[]>();
            this._skillCards = new ulong[3];

            lock (this._lock)
            {
                var characterEquipments = Game.Instance.ItemsRepository.GetCharactersEquips(player.PlayerId).Result;
                foreach (var ce in characterEquipments)
                {
                    ulong[] equips = new ulong[13];
                    ushort characterId = ce.character_id;
                    equips[0] = ce.head;
                    equips[1] = ce.face;
                    equips[2] = ce.body;
                    equips[3] = ce.hands;
                    equips[4] = ce.legs;
                    equips[5] = ce.shoes;
                    equips[6] = ce.back;
                    equips[7] = ce.side;
                    equips[8] = 0; // unk
                    equips[9] = ce.primary;
                    equips[10] = ce.secondary;
                    equips[11] = ce.Throw;
                    equips[12] = ce.melee;
                    this._equips.Add(characterId, equips);
                }
            }
        }


        public ulong[] GetEquipmentByCharacter(ushort characterId)
        {
            lock (this._lock)
            {
                if (!this._equips.ContainsKey(characterId))
                    return new ulong[13];

                return this._equips[characterId];
            }
        }

        public ulong[] GetArmorByCharacter(ushort characterId)
        {
            lock (this._lock)
            {
                if (!this._equips.ContainsKey(characterId))
                    return new ulong[9];

                ulong[] armorOnly = new ulong[9];
                for (int i = 0; i < armorOnly.Length; i++)
                {
                    armorOnly[i] = this._equips[characterId][i];
                }
                return armorOnly;
            }
        }

        public ulong[] GetWeaponsByCharacter(ushort characterId)
        {
            lock (this._lock)
            {
                if (!this._equips.ContainsKey(characterId))
                    return new ulong[4];

                return new ulong[4]
                {
                    this._equips[characterId][9],
                    this._equips[characterId][10],
                    this._equips[characterId][11],
                    this._equips[characterId][12],
                };
            }
        }

        public uint[] GetArmorItemIdsByCharacter(ushort characterId)
        {
            if (this._player == null)
                return new uint[9];

            var armor = GetArmorByCharacter(characterId);
            uint[] armorItemIds = new uint[9];

            lock (this._lock)
            {
                for (int i = 0; i < armor.Length; i++)
                    armorItemIds[i] = this._player.InventoryManager.Get(armor[i]).ItemId;
                return armorItemIds;
            }
        }

        public uint[] GetWeaponItemIdsByCharacter(ushort characterId)
        {
            if (this._player == null)
                return new uint[4];

            var weapons = GetWeaponsByCharacter(characterId);
            uint[] armorItemIds = new uint[4];

            lock (this._lock)
            {
                for (int i = 0; i < weapons.Length; i++)
                    armorItemIds[i] = this._player.InventoryManager.Get(weapons[i]).ItemId;
                return armorItemIds;
            }
        }

        public InventoryCard[] GetSkillCards()
        {
            if (this._player == null)
                return new InventoryCard[3];

            lock (this._lock)
            {
                return new InventoryCard[3]
                {
                    this._player.InventoryManager.Get(this._skillCards[0]),
                    this._player.InventoryManager.Get(this._skillCards[1]),
                    this._player.InventoryManager.Get(this._skillCards[2])
                };
            }
        }

        public void RemoveFunctionCard(ulong cardId)
        {
            lock (this._functionCardlock)
                this._functionCards.Remove(cardId);
        }

        public void UnequipItem(ulong cardId)
        {
            lock (this._lock)
            {
                for (int i = 0; i < this._unlockedCharacters.Count; i++)
                {
                    var characterId = this._unlockedCharacters[i];
                    if (!this._equips.ContainsKey(characterId))
                        continue;

                    for (int j = 0; j < 13; j++)
                        if (this._equips[characterId][j] == cardId)
                            this._equips[characterId][j] = 0;
                }
            }
        }

        public void AddFunctionCard(ulong cardId)
        {
            lock (this._functionCardlock)
                this._functionCards.Add(cardId);
        }

        public void SetFunctionCards(List<ulong> cards)
        {
            lock (this._functionCardlock)
                this._functionCards = cards;
        }

        public void SetSkillCards(List<ulong> cards)
        {
            lock (this._skillCardlock)
            {
                this._skillCards = new ulong[3];
                for (int i = 0; i < this._skillCards.Length && i < cards.Count; i++)
                        this._skillCards[i] = cards[i];
            }
                
        }

        public void SetEquipmentForCharacter(ushort character, ulong[] equip)
        {
            this._equips[character] = equip;
        }

        public void SetWeapons(ushort character, ulong[] weapons)
        {
            var characterId = CharacterIndexToId(character);
            if (this._player == null)
                return;

            lock (this._lock)
            {
                foreach (var weapon in weapons)
                {
                    if (weapon == 0)
                        continue;

                    if (!Game.Instance.WeaponManager.CanEquip(this._player.InventoryManager.Get(weapon).ItemId, characterId))
                        return;
                    if (!this._player.InventoryManager.HasCard(weapon) || this._player.InventoryManager.IsExpired(weapon))
                        return;
                }

                this._equips[characterId][9] = weapons[0];
                this._equips[characterId][10] = weapons[1];
                this._equips[characterId][11] = weapons[2];
                this._equips[characterId][12] = weapons[3];

                this._player.SendLobby(LobbyManager.Instance.SetWeapons(character, weapons));
            }
        }

        public void SetArmor(ushort character, ulong[] armor)
        {
            var characterId = CharacterIndexToId(character);
            if (this._player == null)
                return;

            lock (this._lock)
            {
                foreach (var piece in armor)
                {
                    if (piece == 0)
                        continue;

                    if (!this._player.InventoryManager.HasCard(piece) || this._player.InventoryManager.IsExpired(piece))
                        return;
                }

                for (int i = 0; i < 8; i++)
                    this._equips[characterId][i] = armor[i];

                this._player.SendLobby(LobbyManager.Instance.SetArmor(character, armor));
            }
        }

        public ushort CharacterIndexToId(ushort characterIndex)
        {
            switch (characterIndex)
            {
                default: case 0: return 333;
                case 1: return 343;
                case 2: return 578;
                case 3: return 579;
                case 4: return 850;
                case 5: return 851;
            }
        }

        public bool HasEquipped(ulong cardId)
        {
            foreach (var character in this._unlockedCharacters)
                if (HasEquipped(cardId, character))
                    return true;

            return false;
        }

        public bool HasEquipped(ulong cardId, ushort character)
        {
            var equips = this._equips[character];

            foreach (var equipment in equips)
                if (equipment == cardId)
                    return true;

            foreach (var skillCard in this._skillCards)
                if (skillCard == cardId)
                    return true;

            foreach (var functionCard in this._functionCards)
                if (functionCard == cardId)
                    return true;

            return false;
        }

        public uint GetDefaultWeapon()
        {
            if (this._player == null)
                return 0;

            lock (this._lock)
            {
                var characterId = this._player.Character;
                var weapons = GetWeaponItemIdsByCharacter(characterId);
                foreach (var weapon in weapons)
                    if (weapon != 0)
                        return weapon;
            }
            return 0;
        }

        public bool HasMeleeWeapon()
        {
            if (this._player == null)
                return false;

            lock (this._lock)
            {
                var characterId = this._player.Character;
                var weapons = GetWeaponItemIdsByCharacter(characterId);
                return weapons[3] != 0;
            }
        }

        public ushort GetBaseHealth()
        {
            lock (this._lock)
            {
                if (this._player == null)
                    return 0;

                switch (this._player.Character)
                {
                    case 850:
                    case 851: // Dr Uru & Sai
                        return 125;
                    case 578: // kumma
                        return 150;
                    case 579: // miu miu
                        return 90;
                    case 343:
                    case 333: // Hanna & Ken
                    default:
                        return 100;
                }
            }
        }

        public ushort GetBonusHealth()
        {
            lock (this._lock)
            {
                if (this._player == null)
                    return 0;

                var equip = this._equips[this._player.Character];
                switch (this._player.InventoryManager.Get(equip[6]).ItemId)
                {
                    case 1429407489: // green turtle
                        return 10;
                    case 1429410048: // brown turtle
                    case 1429408256: // sidekick
                    case 1429414144: // wallie
                    case 1429414400: // german
                    case 1429430784: // TGS
                    case 1429431040: // orange
                    case 1429409024: // alpha
                    case 1429412097: // yellow helper
                        return 20;
                    case 1429415424: // novice back
                        return 30;
                    default:
                        return 0;
                }
            }
        }

        public bool HasFunctionCard(uint functionId)
        {
            lock (this._player.Lock)
            {
                if (this._player == null)
                    return false;

                foreach (var functionCard in this._functionCards)
                    if (this._player.InventoryManager.Get(functionCard).ItemId == functionId)
                        return true;
            }

            return false;
        }

        public byte GetExtraAmmoForWeaponIndex(byte index)
        {
            if (index == 3)
                return 0; // melee

            lock (this._player.Lock)
            {
                var equip = this._equips[this._player.Character];
                var itemId = this._player.InventoryManager.Get(equip[6]).ItemId;

                if (index == 0 && itemId == 1429409536)
                    return 1;
                if (index == 0 && itemId == 1429409792)
                    return 1;
                if (index == 0 && itemId == 1429408001)
                    return 1;
            }
            return 0;
        }

        public void FinishRound(RoomSessionPlayer session)
        {
            if (this._player == null)
                return;

            lock (this._player.Lock)
            {
                var character = session.Character;
                var playtime = session.GetPlaytime();

                if (!this._equips.ContainsKey(character))
                    return;
                
                var equips = this._equips[character];
                List<ulong> expired = new List<ulong>();
                for(int i = 0; i < equips.Length; i++)
                    if (this._player.InventoryManager.UseCard(equips[i], playtime))
                        expired.Add(equips[i]);

                foreach(var e in expired)
                    this._player.EquipmentManager.UnequipItem(e);


                expired = new List<ulong>();
                lock (this._functionCardlock)
                {
                    for (int i = 0; i < this._functionCards.Count; i++)
                        if (this._player.InventoryManager.UseCard(this._functionCards[i], playtime))
                            expired.Add(this._functionCards[i]);

                    foreach (var e in expired)
                        this._player.EquipmentManager.RemoveFunctionCard(e);
                }

                var cards = this._player.InventoryManager.List();
                this._player.SendLobby(LobbyManager.Instance.Inventory(cards));

                Save();
            }
        }

        public void Save()
        {
            if (this._player == null)
                return;

            lock (this._player.Lock)
            {
                if (this._player.TestRealm)
                    return;

                foreach (var character in this._unlockedCharacters)
                {
                    Game.Instance.ItemsRepository.UpdateCharactersEquips(this._equips[character], character, this._player.PlayerId).GetAwaiter().GetResult();
                }
            }
        }

        public void Close()
        {
            Save();
        }
    }
}
