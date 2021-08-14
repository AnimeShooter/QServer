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

        public PlayerSkillManager()
        {
            this._lock = new object();
        }

        public void Initialize(RoomSessionPlayer player)
        {
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

            lock(this._player.Lock)
                if (this._player != null)
                    this._player.Post(new GCCard(this._player.Player.PlayerId, this._skillPoints % 100, this._skillPoints / 100));
        }

        public void AddSkillPoints(uint amount = 100)
        {
            this._skillPoints += amount;

            lock (this._player.Lock)
                if (this._player != null)
                    this._player.Post(new GCCard(this._player.Player.PlayerId, this._skillPoints % 100, this._skillPoints / 100));
            
        }

        public void ResetPoints()
        {
            this._skillPoints = 0;

            lock (this._player.Lock)
                if (this._player != null)
                    this._player.Post(new GCCard(this._player.Player.PlayerId, 0, 0));
            
        }

        public uint DrawSkill()
        {
            if (this._player == null)
                return 0;

            lock(this._player.Lock)
            {
                this._drawnSkillCard = this._player.RoomSession.SkillManager.GenerateRandomSkill();
                // TODO
                //this._drawnSkillCard.bind(this._player);
            }

            return 0; // TODO // this._drawnSkillCard.Id;
        }
    }
}
