using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class PlayerSkillManager
    {
        private RoomSessionPlayer _player;
        private Skill _drawnSkillCard;
        private InventoryCard[] _equippedCards;
        private Skill _activeSkillCard;
        private uint _skillPoints;
        private object _lock;

        public PlayerSkillManager(RoomSessionPlayer player)
        {
            this._lock = new object();
            this._player = player;
            this._equippedCards = this._player.Player.EquipmentManager.GetSkillCards();
        }

        public void Tick()
        {

        }

        public void RemoveSkillPoints(uint amount = 100)
        {
            if (this._skillPoints <= amount)
                this._skillPoints = 0;
            else
                this._skillPoints -= amount;
            lock(this._lock)
            {
                //if (this._player != null)
                //    this._player.Post(new GCCard(this._player.Player.PlayerId, this._skillPoints & 100, this._skillPoints / 100)); // TODO
            }
        }

        public void AddSkillPoints(uint amount = 100)
        {

        }

        public void ResetPoints()
        {

        }

        public uint DrawSkill()
        {
            return 0;
        }
    }
}
