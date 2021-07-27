using Qserver.GameServer.Network.Managers;
using Qserver.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class RoomSessionPlayer
    {
        private PlayerEffectManager _effectManager; // TODO
        private PlayerWeaponManager _weaponManager; // TODO
        private PlayerSkillManager _skillManager; // todo
        private PlayerEntityManager _entityManager;

        private Position _position;

        private uint _joinTime;
        private uint _startTime;
        private uint _invincibleRemovalTime;
        private uint _respawnTime;

        private bool _isPlaying; // otherwise waiting
        private bool _isRespawning;
        private bool _isSpectating;

        private ushort _character = 343;
        private byte _team = 0;
        private ushort _health = 100;
        private ushort _baseHealth = 100;
        private ushort _bonusHealth = 100;

        private uint[] _armor = new uint[9];

        private object _bulletLock;
        private object _playerLock;

        private bool _hasQuickRevive;
        private bool _isInvincible;

        private ushort _expRate;
        private ushort _donRate;

        private ushort _exp;
        private ushort _don;

        private ushort _highestStreak;
        private ushort _streak;
        private ushort _kills;
        private ushort _deaths;
        private ushort _score;
        private uint _playTime;
        private uint _highestMultiKill;
        private uint _eventItemPickUps;

        private GameConnection _conn;
        private RoomSession _roomSession;
        public ushort Streak
        {
            get { return this._streak; }
            set { this._streak = value; }
        }
        public ushort Kills
        {
            get { return this._kills; }
            set { this._kills = value; }
        }
        public ushort Deaths
        {
            get { return this._deaths; }
            set { this._deaths = value; }
        }
        public ushort Score
        {
            get { return this._score; }
            set { this._score = value; }
        }
        public uint PlayTime
        {
            get { return this._playTime; }
            set { this._playTime = value; }
        }
        public uint HighestMultiKill
        {
            get { return this._highestMultiKill; }
            set { this._highestMultiKill = value; }
        }
        public uint EventItemPickUps
        {
            get { return this._eventItemPickUps; }
            set { this._eventItemPickUps = value; }
        }

        public bool Invincible
        {
            get { return _isInvincible; }
        }

        public ushort Health
        {
            get { return this._health; }
        }
        public ushort BonusHealth
        {
            get { return this._bonusHealth; }
        }

        public bool Death
        {
            get { return this._health <= 0; }
        }

        public bool Playing
        {
            get { return this._isPlaying; }
        }
        public bool Respawning
        {
            get { return this._isRespawning; }
        }
        public bool Spectating
        {
            get { return this._isSpectating; }
        }

        public ushort Character
        {
            get { return this._character; }
        }
        public Player Player
        {
            get { return this._conn.Player; }
        }
        public Position Position
        {
            get { return this._position; }
        }

        public byte Team
        {
            get { return this._team; }
        }

        public PlayerEffectManager EffectManager
        {
            get { return this._effectManager; }
        }

        public PlayerWeaponManager WeaponManager
        {
            get { return this._weaponManager; }
        }
        public PlayerSkillManager SkillManager
        {
            get { return this._skillManager; }
        }
        public PlayerEntityManager EntityManager
        {
            get { return this._entityManager; }
        }

        public RoomSession RoomSession
        {
            get { return this._roomSession; }
        }

        public RoomSessionPlayer(GameConnection conn, RoomSession roomSession, byte team)
        {
            this._conn = conn;
            this._roomSession = roomSession;
            this._team = team;
            this._isPlaying = false;
            this._isInvincible = false;
            this._isSpectating = false;
            this._streak = 0;
            this._kills = 0;
            this._deaths = 0;
            this._score = 0;
            this._exp = 0;
            this._expRate = 0;
            this._don = 0;
            this._donRate = 0;
            this._playTime = 0;
            this._highestStreak = 0;
            this._highestMultiKill = 0;
            this._eventItemPickUps = 0;

            this._effectManager = new PlayerEffectManager();
            this._weaponManager = new PlayerWeaponManager();
            //this._skillManager = new PlayerSkillManager();
            this._entityManager = new PlayerEntityManager();

            var player = conn.Player;

            this._joinTime = Util.Util.Timestamp();
            this._startTime = this._joinTime + 30;
            this._character = player.Character;

            var equipMgr = player.EquipmentManager;

            this._baseHealth = equipMgr.GetBaseHealth();
            this._bonusHealth = equipMgr.GetBonusHealth();
            this._health = GetDefaultHealth();

            this._armor = equipMgr.GetArmorItemIdsByCharacter(this._character);

            this._hasQuickRevive = equipMgr.HasFunctionCard((uint)Items.QUICK_REVIVE);

            this._expRate += equipMgr.HasFunctionCard((uint)Items.EXP_MAKER_25) ? (ushort)25 : (ushort)0;
            this._expRate += equipMgr.HasFunctionCard((uint)Items.EXP_MAKER_50) ? (ushort)50 : (ushort)0;

            this._donRate += equipMgr.HasFunctionCard((uint)Items.DON_MAKER_25) ? (ushort)25 : (ushort)0;
            this._donRate += equipMgr.HasFunctionCard((uint)Items.DON_MAKER_50) ? (ushort)50 : (ushort)0;
        }

        public void Tick()
        {
            if (CanStart())
            {
                Start();
                //this._roomSession.SpawnPlayer(this);
            }

            if (this._isPlaying)
            {
                //this._effectManager.Tick();
                this._skillManager.Tick();
            }

            var removeInvincible = this._invincibleRemovalTime <= Util.Util.Timestamp() && this._isInvincible;
            if (removeInvincible)
                RemoveInvincibility();

            var needRespawn = this._respawnTime <= Util.Util.Timestamp() && this._isRespawning;
            if (needRespawn)
                Respawn();

        }

        public void Start()
        {
            this._isPlaying = true;

            this._conn.Player.AchievementContainer.ResetRecent();

            //this._roomSession.RelayExcept<GCGameState>(this._conn.Player.PlayerId, this._conn.Player.PlayerId, 3);
            //this._conn.PostNetEvent(new GCGameState(this._conn.Player.PlayerId), 4);

            //this._roomSession.SyncPlayer(this);
        }

        public void Stop()
        {
            //this._effectManager.Clear();
            this._entityManager.Close();

            var player = this._conn.Player;

            player.EquipmentManager.FinishRound(this);
            player.StatsManager.Apply(this);

            Game.Instance.LevelManager.OnPlayerFinish(this);
            Game.Instance.AchievementManager.OnPlayerFinish(this);

            player.Update();
            player.SendLobby(LobbyManager.Instance.UpdateAccount(player));
        }

        public bool CanStart()
        {
            var currTime = Util.Util.Timestamp();
            return this._startTime <= currTime && !this._isPlaying;
        }

        public void MakeInvincible()
        {
            this._isInvincible = true;
            this._invincibleRemovalTime = Util.Util.Timestamp() + 5;
        }

        public void RemoveInvincibility()
        {
            this._isInvincible = false;
            //this._roomSession.RelayPlaying<GCGameState>(this._conn.Player.PlayerId, 8);
        }

        public void AddPlayer(RoomSessionPlayer player)
        {
            //this._conn.AddSession(player);
        }

        public void AddHealth(ushort health, bool updateClient = false)
        {
            if (this._health > this._baseHealth + this._bonusHealth)
                return;

            if(this._health + health > this._baseHealth + this._bonusHealth)
            {
                SetHealth((ushort)(this._baseHealth + this._bonusHealth), updateClient);
                return;
            }

            SetHealth(this._health += health, updateClient);
        }

        public void TakeHealth(ushort health, bool updateClient = false)
        {
            if (health > this._health)
                SetHealth(0, updateClient);
            else
                SetHealth((ushort)(this._health - health), updateClient);
        }

        public void SetHealth(ushort health, bool updateClient = false)
        {
            this._health = health;
            if (updateClient)
                Post(new GCGameState(this._conn.Player.PlayerId, 16, this._health));
        }

        public void Respawn()
        {
            this._isRespawning = true;
            this._roomSession.SpawnPlayer(this);
        }

        public void StartRespawnCooldown()
        {
            this._isRespawning = true;
            this._skillManager.ResetPoints();

            //var cooldown = GetRespawnCooldown();

            //this._respawnTime = Util.Util.Timestamp() + cooldown;
            //Post(new GCGameState(this._conn.Player.PlayerId, 29, cooldown * 1000));
        }

        public void AddEventItemPickup()
        {
            this._eventItemPickUps++;
        }

        public byte GetRespawnCooldown()
        {
            return this._hasQuickRevive ? (byte)5 : (byte)10;
        }

        public uint GetDon()
        {
            uint don = 0;
            uint playtimeDon = GetPlaytime() / 8;
            if (playtimeDon > 250)
                playtimeDon = 250;

            don += (uint)(18 * this._kills);
            don += (uint)(7 * this._kills);
            don += (uint)(10 * this._eventItemPickUps);
            don += playtimeDon;

            if (this._roomSession.GameMode.IsMissionMode())
                don += this._score;
            float bonus = this._donRate / 100f;
            don += (uint)(don * bonus);
            return don;
        }

        public void ResetStreak()
        {
            this._streak = 0;
        }

        public void AddStreak()
        {
            this._streak++;
            if (this._streak > this._highestStreak)
                this._highestStreak = this._streak;
        }

        public void HealTeam(uint healing)
        {
            //var players = this._roomSession.GetPlayersFromTeam(this._team);

            //foreach(var p in players)
            //{
            //    if (p.Dead)
            //        continue;

            //    p.AddHealth(50, true);
            //    if (p.Player.PlayerId != p.PlayerId)
            //        p.Post(new GCGameItem(1, p.Player.PlayerId, 1191182350, null));
            //}
        }

        public uint GetPlaytime()
        {
            var currTime = Util.Util.Timestamp();
            if (currTime < this._startTime)
                return 0;

            return currTime = this._startTime;
        }

        public void Post(GameNetEvent e)
        {
            if(this._conn == null)
            {
                // Dispose
                return;
            }

            try
            {
                this._conn.PostNetEvent(e);
            }
            catch(Exception ex)
            {
                Log.Message(LogType.ERROR, ex.ToString());
            }
        }

        public uint GetExperience()
        {
            uint exp = 0;
            uint playExp = GetPlaytime() / 4;

            if (playExp >= 500)
                playExp = 500;

            exp += (uint)(35 * this._kills);
            exp += (uint)(10 * this._deaths);
            exp += (uint)(10 * this._eventItemPickUps);
            exp += playExp;

            //if (this._roomSession.GameMode.IsMissionMode())
            //    exp += this._score;
            float bonus = this._expRate / 100f;
            exp += (uint)(exp * bonus);
            return exp;
        }

        public void Initialize()
        {
            this._isRespawning = false;
            //this._effectManager.Initialize(); // TODO TODO TODO TODO
            //this._weaponManager.Initialize();
            //this._skillManager.Initialize();
            //this._entityManager.Initialize();
        }

        public void AddKill()
        {
            this._kills++;
        }

        public void AddDeath()
        {
            this._deaths++;
        }

        public void AddScore(ushort score = 1)
        {
            this._score += score;
            //this._roomSession.AddPointsForTeam(this._team, score);
        }

        public ushort GetDefaultHealth()
        {
            return (ushort)(this._baseHealth + this._bonusHealth);
        }


    }
}
