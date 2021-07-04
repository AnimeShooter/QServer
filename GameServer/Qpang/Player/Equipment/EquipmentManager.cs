using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class EquipmentManager
    {
        private Dictionary<ushort, ulong[]> _equips; // long[13]
        private List<ushort> _unlockedCharacters;
        private ulong[] _skillCards; // long[3]
        private object _lock;
        private Player _player;
        private List<ulong> _functionCards;
        private object _functionCardlock;

        public List<ushort> UnlockerCharacters
        {
            get { return _unlockedCharacters; }
        }


        public EquipmentManager(Player player)
        {
            this._player = player;
            this._unlockedCharacters = new List<ushort>() { 333, 343, 578, 578, 850, 851 };

            // TODO: database get player equipments


        }

       
        public ulong[] GetEquipmentByCharacter(ushort characterId)
        {
            lock(this._lock)
            {
                // unk
            }
            return null;
        }

        public void Save()
        {
            // TODO: database
        }
        public void Close()
        {
            Save();
        }
    }
}
