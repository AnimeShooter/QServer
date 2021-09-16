using Qserver.GameServer.Network.Managers;
using Qserver.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class RoomSessionPlayer
    {
        private PlayerEffectManager _effectManager; // TODO: buggfix effect expiring
        private PlayerWeaponManager _weaponManager;
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
        private uint _highestMultiKill;
        private uint _eventItemPickUps;

        private GameConnection _conn;
        private RoomSession _roomSession;

        private List<long> _weaponReloads;
        private List<long> _weaponReswaps;

        private uint _firstSeen;


        private List<float[]> samples = new List<float[]>();
        private DateTime start;
        private float[] lastTick;

        private DateTime _lastMoveStart;
        private uint _lastCmd;
        private float _startX;
        private float _startY;
        private float _startZ;

        public object Lock
        {
            get { return this._playerLock; }
        }
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
            get { return GetPlaytime(); }
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
            get { return this._isInvincible; }
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
            set { this._isSpectating = value; }
        }
        public ushort ExpRate
        {
            get { return this._expRate; }
        }
        public ushort DonRate
        {
            get { return this._donRate; }
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
            set { this._position = value; }
        }
        public byte Team
        {
            get { return this._team; }
        }
        public bool IsBot
        {
            get { return IsRobot(); }
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
        public uint FirstSeen
        {
            get { return this._firstSeen; }
            set { this._firstSeen = value; }
        }

        public RoomSessionPlayer(GameConnection conn, RoomSession roomSession, byte team)
        {
            this._playerLock = new object();

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
            this._highestStreak = 0;
            this._highestMultiKill = 0;
            this._eventItemPickUps = 0;

            this._effectManager = new PlayerEffectManager();
            this._weaponManager = new PlayerWeaponManager();
            this._skillManager = new PlayerSkillManager(); // TODO
            this._entityManager = new PlayerEntityManager();

            var player = conn.Player;

            this._joinTime = Util.Util.Timestamp();
            this._startTime = this._joinTime + 5;
            //this._startTime = this._joinTime + 30;
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

            this._weaponReloads = new List<long>();
            this._weaponReswaps = new List<long>();
        }

        public virtual void Tick()
        {
            if (CanStart())
            {
                Start();
                this._roomSession.SpawnPlayer(this);
            }

            if (this._isPlaying)
            {
                this._effectManager.Tick();
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

            this._roomSession.RelayExcept<GCGameState>(this._conn.Player.PlayerId, this._conn.Player.PlayerId, (uint)3, (uint)0, (uint)0);
            this._conn.PostNetEvent(new GCGameState(this._conn.Player.PlayerId, (uint)4));

            this._roomSession.SyncPlayer(this);
        }

        public void Stop()
        {
            this._effectManager.Clear();
            this._entityManager.Close();

            var player = this._conn.Player;

            player.EquipmentManager.FinishRound(this);
            player.StatsManager.Apply(this);

            Game.Instance.LevelManager.OnPlayerFinish(this);
            Game.Instance.AchievementManager.OnPlayerFinish(this);

            player.Update();
            player.SendLobby(LobbyManager.Instance.UpdateAccount(player));
            //player.SendLobby(LobbyManager.Instance.Inventory(player.InventoryManager.List()));

            // TODO TEST anti-cheating
            //Console.WriteLine("Reload history of " + this._conn.Player.Name);
            //for(int i = 0; i < this._weaponReloads.Count; i++)
            //{
            //    long nearby = long.MaxValue;
            //    foreach(var x in this._weaponReswaps)
            //    {
            //        if (x > this._weaponReloads[i] && x - this._weaponReloads[i] < 5000 && x < nearby)
            //            nearby = x;
            //    }
            //    Console.WriteLine($"Reload[{i}]: {this._weaponReloads[i].ToString().PadLeft(8)} - {nearby.ToString().PadLeft(8)} = {(nearby-this._weaponReloads[i]).ToString().PadLeft(8)}ms");
            //}
        }

        public bool CanStart()
        {
            var currTime = Util.Util.Timestamp();
            return this._startTime + (IsBot ? 2 : 0) <= currTime && !this._isPlaying; // TODO: find out why sync didnt work
        }

        public void MakeInvincible(uint duration = 5)
        {
            this._isInvincible = true;
            this._invincibleRemovalTime = Util.Util.Timestamp() + duration;
        }

        public void RemoveInvincibility()
        {
            this._isInvincible = false;
            this._roomSession.RelayPlaying<GCGameState>(this._conn.Player.PlayerId, (uint)8, (uint)0, (uint)0);
        }

        public void AddPlayer(RoomSessionPlayer player)
        {
            this._conn.AddSession(player);
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
            this._isRespawning = false;
            this._roomSession.SpawnPlayer(this);

        }

        public void StartPrespawn() // Pre respawn (countdown)
        {
            this._isRespawning = true;
            //this._skillManager.ResetPoints(); // NOTE: dont reset after death?

            var cooldown = GetRespawnCooldown();

            this._respawnTime = Util.Util.Timestamp() + cooldown;
            Post(new GCGameState(this._conn.Player.PlayerId, 29, (uint)(cooldown * 1000)));
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
            var players = this._roomSession.GetPlayersForTeam(this._team);

            foreach (var p in players)
            {
                if (p.Death)
                    continue;

                p.AddHealth(50, true);
                if (p.Player.PlayerId != this._conn.Player.PlayerId)
                    p.Post(new GCGameItem(1, p.Player.PlayerId, 1191182350, 0));
            }
        }

        public uint GetPlaytime()
        {
            var currTime = Util.Util.Timestamp();
            if (currTime < this._startTime)
                return 0;

            return currTime - this._startTime;
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

            if (this._roomSession.GameMode.IsMissionMode())
                exp += this._score;
            float bonus = this._expRate / 100f;
            exp += (uint)(exp * bonus);
            return exp;
        }

        public void Initialize()
        {
            this._isRespawning = false;
            this._effectManager.Initialize(this); // TODO TODO TODO TODO
            this._weaponManager.Initialize(this);
            this._skillManager.Initialize(this);
            this._entityManager.Initialize(this);
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
            this._roomSession.AddPointsForTeam(this._team, score);
        }

        public ushort GetDefaultHealth()
        {
            return (ushort)(this._baseHealth + this._bonusHealth);
        }

        public virtual bool IsRobot()
        {
            return false;
        }

        // TEST anti-cheating
        public void UpdateCoords(Spawn coords)
        {
            UpdateCoords(new Position()
            {
                X = coords.X,
                Y = coords.Y,
                Z = coords.Z
            });
        }
        public void UpdateCoords(Position coords)
        {
            this._position = coords;
        }

        public bool IsInRange(Position target, float maxDistance, bool is3D)
        {
            return IsInRange(new Spawn()
            {
                X = target.X,
                Y = target.Y,
                Z = target.Z
            }, maxDistance, is3D);
        }

        public bool IsInRange(Spawn target, float maxDistance, bool is3D)
        {
            float dist = is3D ? ((target.X - this._position.X) * (target.X - this._position.X)) +
                    ((target.Y - this._position.Y) * (target.Y - this._position.Y)) +
                    ((target.Z - this._position.Z) * (target.Z - this._position.Z))
                    : 
                    ((target.X - this._position.X) * (target.X - this._position.X)) +
                    ((target.Y - this._position.Y) * (target.Y - this._position.Y));

            return dist < maxDistance;
        }
        public void OnReload()
        {
            this._weaponReloads.Add(Environment.TickCount64);
        }
        public void OnReswap()
        {
            this._weaponReswaps.Add(Environment.TickCount64);
        }

        public void SpeedTest(uint cmd, float x, float y, float z)
        {
            // NOTE: max speed 1.05 @ 500ms (2.1/s)
            if (cmd == 0 && this._lastMoveStart != DateTime.MinValue)
            {
                var currTime = DateTime.UtcNow;
                // calc speed

                // TODO set speed variable based on last CMD

                float distance = ((this._startX - x) * (this._startX - x)) +
                                 ((this._startY - y) * (this._startY - y)) +
                                 ((this._startZ - z) * (this._startZ - z));

                TimeSpan timeframe = (currTime - this._lastMoveStart);

                double vPerSec = (distance / 1000f / timeframe.TotalMilliseconds) * 1000f;

                Console.WriteLine($"{vPerSec}/s");

                this._lastMoveStart = DateTime.MinValue;
                this._lastCmd = 0;
            }else if(this._lastCmd == 0 && cmd != 0 && this._lastMoveStart == DateTime.MinValue)
            {
                this._startX = x;
                this._startY = y;
                this._startZ = z;
                this._lastMoveStart = DateTime.UtcNow;
            }
        }

        public void Test(uint cmd, float x, float y, float z)
        {
            if (cmd == 0)
            {
                var now = DateTime.UtcNow;
                float distance = 0;
                for (int i = 0; i < this.samples.Count - 1; i++)
                {
                    distance += (
                        ((samples[i][0] - samples[i + 1][0]) * (samples[i][0] - samples[i + 1][0])) +
                        ((samples[i][1] - samples[i + 1][1]) * (samples[i][1] - samples[i + 1][1])) +
                        ((samples[i][2] - samples[i + 1][2]) * (samples[i][2] - samples[i + 1][2]))
                    );
                }
                var duration = now - start;
                Console.WriteLine($"Distance walked: {distance} over {(duration).TotalMilliseconds} time ({(distance / duration.TotalMilliseconds) / 1000f}/s)");
                this.samples.Clear();
                start = DateTime.MinValue;
            }
            else if (start == DateTime.MinValue)
                start = DateTime.UtcNow;

            float nowDistance = lastTick == null ? 0 : ((lastTick[0] - x) * (lastTick[0] - x)) +
                                ((lastTick[1] - y) * (lastTick[1] - y)) +
                                ((lastTick[2] - z) * (lastTick[2] - z));

            Console.WriteLine("Last tick distance: " + nowDistance);

            lastTick = new float[]
            {
                x, y, z
            };

            this.samples.Add(lastTick);

        }
    }
}
