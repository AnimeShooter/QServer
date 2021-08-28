using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class PlayerSkillManager
    {
        private RoomSessionPlayer _player;
        private Skill _drawnSkillCard;
        private InventoryCard[] _equippedCards; // TODO: use!!
        private Skill _activeSkillCard;
        private uint _skillPoints;
        private uint _lastTick;
        private object _lock;

        public PlayerSkillManager()
        {
            this._lock = new object();
        }

        public Skill ActiveSkillCard
        {
            get { return this._activeSkillCard; }
            set { this._activeSkillCard = value; }
        }

        public void Initialize(RoomSessionPlayer player)
        {
            this._player = player;
            this._equippedCards = this._player.Player.EquipmentManager.GetSkillCards();
        }

        public void Tick()
        {
            var currTime = Util.Util.Timestamp();

            if (currTime <= this._lastTick)
                return;

            this._lastTick = currTime;

            if (this._activeSkillCard == null)
                return;

            // do skill action if needed
            switch((Items)this._activeSkillCard.Id)
            {
                case Items.SKILL_VITAL:
                    foreach (var p in this._player.RoomSession.GetPlayersForTeam(this._player.Team))
                        if(this._player.Player.PlayerId != p.Player.PlayerId)
                            p.AddHealth(5, true);
                    break;
            }

            if(this._activeSkillCard.Duration != 99 && this._activeSkillCard.StartTime + this._activeSkillCard.Duration < currTime)
            {
                // uint playerId, uint targetId, byte cmd, uint cardType, uint itemId, ulong seqId
                DisableSkill();
            }
        }

        public void DisableSkill()
        {
            if (this._activeSkillCard == null)
                return;

            lock (this._player.Lock)
            {
                // 4 : activate
                // 9 : stop?
                // 2 : remove?

                // Send expire to others
                this._player.RoomSession.RelayPlayingExcept<GCCard>(this._player.Player.PlayerId, this._player.Player.PlayerId, (uint)0, (byte)4, (uint)9, this._activeSkillCard.Id, (ulong)0);

                // expire self (2 too?)
                //this._player.Post(new GCCard(this._player.Player.PlayerId, (uint)0, (byte)4, (uint)9, this._activeSkillCard.Id, (ulong)0));
                this._player.Post(new GCCard(this._player.Player.PlayerId, (uint)0, (byte)4, (uint)9, this._activeSkillCard.Id, (ulong)this._activeSkillCard.Id));
                //this._player.Post(new GCCard(this._player.Player.PlayerId, (uint)0, (byte)2, (uint)9, this._activeSkillCard.Id, (ulong)0));
                //this._player.Post(new GCCard(this._player.Player.PlayerId, (uint)0, (byte)2, (uint)9, this._activeSkillCard.Id, (ulong)this._activeSkillCard.Id));
                //this._player.Post(new GCCard(this._player.Player.PlayerId, (uint)0, (byte)1, (uint)9, this._activeSkillCard.Id, (ulong)0));
                //this._player.Post(new GCCard(this._player.Player.PlayerId, (uint)0, (byte)1, (uint)9, this._activeSkillCard.Id, (ulong)this._activeSkillCard.Id));

                //this._player.RoomSession.RelayPlaying<GCCard>(this._player.Player.PlayerId, (uint)0, (byte)9, (uint)9, this._activeSkillCard.Id, (ulong)this._activeSkillCard.Id); 

                // test 1, msgbox
                //this._player.RoomSession.RelayPlaying<GCCard>(this._player.Player.PlayerId, (uint)0, (byte)1, (uint)9, this._activeSkillCard.Id, (ulong)this._activeSkillCard.Id); 

            }
                
            this._activeSkillCard = null;
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
                this._drawnSkillCard.Bind(this._player);
            }

            return this._drawnSkillCard.Id;
        }

        public void UseSkill(uint uid, uint targetUid, byte cmd, uint cardType, uint itemId, ulong seqId)
        {
            if (this._player == null)
                return;

            // Character ability
            if(cardType == 0x07)
            {
                // TODO check character
                lock (this._player.Lock)
                    this._player.RoomSession.RelayPlaying<GCCard>(uid, targetUid, (byte)cmd, cardType, itemId, seqId);

                return;
            }

            // Skill card
            var skill = Game.Instance.SkillManager.GetSkill(itemId);
            if (skill == null)
                return; // skill not exits?

            // TODO check
            if (this._skillPoints < 100)
                return;

            // assume it all costs 100
            RemoveSkillPoints(100);

            var target = this._player;

            // TODO: get target
            //if (targetUid != 0)
            //    Game.Instance.Get

            lock (this._player.Lock)
            {
                skill.OnUse(target);
                this._player.RoomSession.RelayPlaying<GCCard>(uid, targetUid, (byte)cmd, cardType, itemId, seqId);
                this._activeSkillCard = skill;
            }
        }
    }
}
