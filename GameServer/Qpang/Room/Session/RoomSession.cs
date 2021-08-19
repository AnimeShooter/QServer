using System;
using System.Collections.Generic;
using System.Linq;

namespace Qserver.GameServer.Qpang
{
    public class RoomSession
    {
        private object _lockPlayers;
        private object _lockLeavers;

        private Room _room;

        private bool _isFinished;
        private bool _isPoints;

        private uint _goal;
        private uint _bluePoints;
        private uint _yellowPoints;

        private uint _startTime;
        private uint _endTime;
        private uint _lastTickTime;

        private uint _essenceDropTime;
        private bool _isEssenceReset;

        private GameMode _gameMode;
        private GameItemManager _itemManager;
        private RoomSkillManager _skillManager;

        private Dictionary<uint, RoomSessionPlayer> _players;

        private List<RoomSessionPlayer> _leavers;

        private RoomSessionPlayer _essenceHolder;
        private Spawn _essencePosition = new Spawn() { X = 0, Y = 0, Z = 0 };

        private RoomSessionPlayer _blueVIP;
        private RoomSessionPlayer _nexBlueVIP;
        private uint _blueVIPSetTime;
        private RoomSessionPlayer _yellowVIP;
        private RoomSessionPlayer _nexYellowVIP;
        private uint _yellowVIPSetTime;

        public RoomSessionPlayer BlueVIP
        {
            get { return this._blueVIP; }
        }
        public RoomSessionPlayer YellowVIP
        {
            get { return this._yellowVIP; }
        }
        public uint BluePoints
        {
            get { return this._bluePoints; }
        }
        public uint YellowPoints
        {
            get { return this._yellowPoints; }
        }
        public bool IsEssenceReset
        {
            get { return this._isEssenceReset; }
        }
        public bool IsEssenceDropped
        {
            get { return this._essenceHolder == null; }
        }
        public GameMode GameMode
        {
            get { return this._gameMode; }
        }

        public GameItemManager ItemManager
        {
            get { return this._itemManager; }
        }

        public RoomSkillManager SkillManager
        {
            get { return this._skillManager; }
        }

        public bool Finished
        {
            get { return this._isFinished; }
        }
        public Room Room
        {
            get { return this._room; }
        }

        public RoomSessionPlayer EssenceHolder
        {
            get { return this._essenceHolder; }
            set
            {
                this._essenceHolder = value;
                if (value != null)
                    this._essenceDropTime = uint.MaxValue;
                else
                    this._essenceDropTime = Util.Util.Timestamp();
            }
        }

        public Spawn EssencePosition
        {
            get { return this._essencePosition; }
            set { this._essencePosition = value; }
        }

        public RoomSession(Room room, GameMode mode)
        {
            this._lockPlayers = new object();
            this._lockLeavers = new object();

            this._room = room;
            this._gameMode = mode;
            this._isFinished = false;
            this._essenceHolder = null;
            this._bluePoints = 0;
            this._yellowPoints = 0;
            this._lastTickTime = uint.MaxValue;
            this._essenceDropTime = uint.MaxValue;
            this._blueVIP = null;
            this._nexBlueVIP = null;
            this._blueVIPSetTime = uint.MaxValue;
            this._yellowVIP = null;
            this._nexYellowVIP = null;
            this._yellowVIPSetTime = uint.MaxValue;

            this._leavers = new List<RoomSessionPlayer>();
            this._players = new Dictionary<uint, RoomSessionPlayer>();

            this._itemManager = new GameItemManager();
            this._skillManager = new RoomSkillManager(); // TODO 

            this._goal = this._room.PointsGame ? this._room.ScorePoints : this._room.ScoreTime;
            this._isPoints = this._room.PointsGame;
            this._startTime = Util.Util.Timestamp() + 30 + 5; // waiting for players + countdown TODO 30+5
            this._endTime = this._room.PointsGame ? uint.MaxValue : this._startTime + (this._room.ScoreTime * 60);
        }

        public void Initialize()
        {
            this._itemManager.Initialize(this);
            this._skillManager.Initialize(this);

            this._gameMode.OnStart(this);
            this._essencePosition = new Spawn()
            {
                X = 0,
                Y = 0,
                Z = 0
            };// TODO: Game.Instance.SpawnManager.GetEssenceSpawn(this._room.Map);
        }

        public void AddPlayer(GameConnection conn, byte team)
        {
            var player = new RoomSessionPlayer(conn, this, team);

            if (conn.Player.RoomPlayer != null)
                conn.Player.RoomPlayer.RoomSessionPlayer = player;

            player.Initialize();
            player.Spectating = conn.Player.RoomPlayer.Spectating;

            lock (this._lockPlayers)
            {
                foreach (var p in this._players)
                {
                    if (!p.Value.Spectating)
                        player.AddPlayer(p.Value);

                    if (!player.Spectating)
                        p.Value.AddPlayer(player);

                    if (p.Value.Playing)
                        player.Post(new GCGameState(p.Key, 3));
                }
                this._players[player.Player.PlayerId] = player; // add or set?
            }

            var spawn = Game.Instance.SpawnManager.GetRandomSpawn(this._room.Map, team);
            player.Post(new GCGameState(player.Player.PlayerId, 11, 0)); // waiting for players
            player.Post(new GCRespawn(player.Player.PlayerId, player.Character, 1, spawn.X, spawn.Y, spawn.Z));

            if (!player.Spectating)
            {

                // TODO verify Hadnle Leaving

                lock (this._lockLeavers)
                {
                    RoomSessionPlayer found = null;
                    foreach (var p in this._leavers)
                        if (p.Player.PlayerId == player.Player.PlayerId)
                            found = p;

                    if (found != null)
                        this._leavers.Remove(found);
                }
            }
        }

        public bool RemovePlayer(uint id)
        {
            RoomSessionPlayer player = null;
            lock (this._lockPlayers)
            {

                if (!this._players.ContainsKey(id))
                    return false;

                player = this._players[id];

                if (player == this._essenceHolder)
                {
                    SetEssenceHolder(null);
                    var pos = player.Position;
                    foreach (var p in this._players)
                        p.Value.Post(new GCHitEssence(p.Value.Player.PlayerId, p.Value.Player.PlayerId, 3, pos.X, pos.Y, pos.X, 0, 6));
                }

                if (player == this._blueVIP)
                    this._blueVIP = null;
                else if (player == this._yellowVIP)
                    this._yellowVIP = null;

                if (player.Playing)
                    Relay<GCGameState>(id, (uint)15, (uint)0, (uint)0);
                else
                {
                    RelayPlaying<GCGameState>(id, (uint)15, (uint)0, (uint)0);
                    player.Post(new GCGameState(id, 15));
                }

                this._players.Remove(id);
            }

            if (player.Spectating)
                return true;

            lock (this._lockLeavers)
                this._leavers.Add(player);

            // see if match should be ended due to leaving
            if (!this._isFinished)
            {
                if (this._gameMode.IsTeamMode())
                {
                    // finish empty team PvP
                    if (this._players.Count == 0)
                        Finish();
                    else
                    {
                        var bluePlayers = GetPlayersForTeam(1);
                        var yellowPlayers = GetPlayersForTeam(2);

                        // Finish solo team PvP
                        if (bluePlayers.Count == 0)
                        {
                            this._yellowPoints = 1;
                            this._bluePoints = 0;
                            Finish();
                        }
                        else if (yellowPlayers.Count == 0)
                        {
                            this._yellowPoints = 0;
                            this._bluePoints = 1;
                            Finish();
                        }
                    }
                }
                else if(this._gameMode.IsPvE())
                {
                    // finish empty PvE room
                    if (this._players.Count == 0)
                        Finish();
                }
                else
                {
                    // finish solo deathmath PvP
                    if (this._players.Count <= 1)
                        Finish();
                }
            }
            return true;
        }

        public RoomSessionPlayer Find(uint playerId)
        {
            lock (this._lockPlayers)
                if (this._players.ContainsKey(playerId))
                    return this._players[playerId];

            return null;
        }

        public List<RoomSessionPlayer> GetPlayers()
        {
            lock (this._lockPlayers)
            {
                List<RoomSessionPlayer> players = new List<RoomSessionPlayer>();
                foreach (var p in this._players)
                    players.Add(p.Value);
                return players;
            }
        }

        public List<RoomSessionPlayer> GetPlayersForTeam(byte team)
        {
            lock (this._lockPlayers)
            {
                List<RoomSessionPlayer> players = new List<RoomSessionPlayer>();
                foreach (var p in this._players)
                {
                    if (p.Value.Team == team && !p.Value.Spectating)
                        players.Add(p.Value);
                }
                return players;
            }
        }

        public void HandlePlayerFinished(RoomSessionPlayer player)
        {
            player.Player.AddExp(player.GetExperience());
            player.Player.AddDon(player.GetDon());

            bool isTeamMode = this._gameMode.IsTeamMode();
            bool isMissionMode = this._gameMode.IsMissionMode();

            var statsMgr = player.Player.StatsManager;

            if (isTeamMode)
            {
                var yellowPoints = this._yellowPoints;
                var bluePoints = this._bluePoints;

                if (yellowPoints == bluePoints)
                {
                    // draw
                    if (isMissionMode)
                        statsMgr.AddMissionDraw();
                    else
                        statsMgr.AddNormalDraw();
                }
                else
                {
                    var won = bluePoints > yellowPoints ? player.Team == 1 : player.Team == 2;

                    if (won)
                    {
                        if (isMissionMode)
                            statsMgr.AddMissionWin();
                        else
                            statsMgr.AddNormalWin();
                    }
                    else
                    {
                        if (isMissionMode)
                            statsMgr.AddMissionLoss();
                        else
                            statsMgr.AddNormalLoss();
                    }
                }
            }
            else
            {
                var players = GetPlayers();
                var totalWinners = players.Count % 2 == 0 ? players.Count / 2 : (players.Count - 1) / 2;

                // TODO: sort players and pick the first totalWinners with highest score?

                for (int i = 0; i < totalWinners % players.Count; i++)
                {
                    if (player.Player.PlayerId == players[i].Player.PlayerId)
                    {
                        if (isMissionMode)
                            player.Player.StatsManager.AddMissionWin();
                        else
                            player.Player.StatsManager.AddNormalWin();
                    }
                    else if (i == totalWinners - 1)
                    {
                        // Losers here
                        if (isMissionMode)
                            player.Player.StatsManager.AddMissionLoss();
                        else
                            player.Player.StatsManager.AddNormalLoss();
                    }
                }
            }
        }

        public void Finish()
        {
            if (this._isFinished)
                return;

            this._isFinished = true;
            this._itemManager.Reset();
            this._itemManager = new GameItemManager();
            this._essenceHolder = null;
            this._nexBlueVIP = null;
            this._blueVIP = null;
            this._nexYellowVIP = null;
            this._yellowVIP = null;

            lock (this._lockLeavers)
            {
                foreach (var p in this._leavers)
                {
                    if (this._gameMode.IsMissionMode())
                        p.Player.StatsManager.AddMissionLoss();
                    else
                        p.Player.StatsManager.AddNormalLoss();
                    p.Stop();
                }
            }

            var players = GetPlayers();
            foreach (var p in players)
            {
                if (!p.Spectating)
                {
                    HandlePlayerFinished(p);
                    p.Stop();
                }
            }

            var playingPlayers = GetPlayingPlayers();
            // TODO: sort by score

            foreach(var player in players)
            {
                var p = player.Player;

                player.Post(new GCGameState(p.PlayerId, 1));
                player.Post(new GCGameState(p.PlayerId, 23));
                player.Post(new GCScoreResult(this, playingPlayers));
            }

            this._room.Finish();
        }

        public bool CanFinish()
        {
            if (this._isFinished)
                return false;

            if (this._isPoints)
            {
                if (this._gameMode.IsTeamMode())
                {
                    var bluePoints = this._bluePoints;
                    var yellowPoints = this._yellowPoints;

                    return bluePoints >= this._goal || yellowPoints >= this._goal;
                }
                return GetTopScore() >= this._goal;
            }

            return Util.Util.Timestamp() >= this._endTime;
        }

        public uint GetTopScore()
        {
            var players = GetPlayers();

            uint higherScore = 0;

            foreach (var p in players)
                if (p.Score > higherScore)
                    higherScore = p.Score;

            return higherScore;
        }

        public void Tick()
        {
            if (!this._isFinished)
            {
                lock (this._lockPlayers)
                {
                    this._itemManager.Tick();
                    this._gameMode.Tick(this);

                    foreach (var p in this._players)
                        p.Value.Tick();
                }
            }

            if (CanFinish())
                Finish();
        }

        public void Clear()
        {
            lock (this._lockPlayers)
                this._players.Clear();

            lock (this._lockLeavers)
                this._leavers.Clear();
        }

        public bool IsAlmostFinished()
        {
            if (this._isPoints)
            {
                var maxDiff = this._goal > 20 ? 10 : 5;
                if (this._gameMode.IsTeamMode())
                    return this._bluePoints + maxDiff >= this._goal || this._yellowPoints + maxDiff >= this._goal;

                var killLeader = this.GetTopScore();
                return killLeader + maxDiff >= this._goal;
            }

            return Util.Util.Timestamp() + 60 >= this._endTime;
        }

        public void AddPointsForTeam(byte team, uint amount)
        {
            if (team == 1)
                this._bluePoints += amount;
            else
                this._yellowPoints += amount;
        }

        public uint GetPointsForTeam(byte team)
        {
            var players = GetPlayers();
            uint total = 0;

            foreach (var p in players)
                if (p.Team == team)
                    total += p.Score;

            return total;
        }

        public List<RoomSessionPlayer> GetPlayingPlayers()
        {
            lock (this._lockPlayers)
            {
                var players = new List<RoomSessionPlayer>();
                foreach (var p in this._players)
                    if (p.Value.Playing && !p.Value.Spectating)
                        players.Add(p.Value);

                return players;
            }
        }

        public void ResetEssence()
        {
            this._isEssenceReset = true;

            var pos = Game.Instance.SpawnManager.GetEssenceSpawn(this._room.Map);
            var players = GetPlayingPlayers();

            this._essencePosition = pos;

            foreach (var p in players)
                p.Post(new GCHitEssence(p.Player.PlayerId, p.Player.PlayerId, 3, pos.X, pos.Y, pos.Z, 0, 6));
        }

        public void SetEssenceHolder(RoomSessionPlayer player)
        {
            this._essenceHolder = player;

            if (player != null)
            {
                this._essenceDropTime = uint.MaxValue;
                this._isEssenceReset = false;
            }
            else
                this._essenceDropTime = Util.Util.Timestamp();
        }

        public uint GetElapsedEssenceDropTime()
        {
            if (this._essenceDropTime == uint.MaxValue)
                return 0;

            return Util.Util.Timestamp() - this._essenceDropTime;
        }

        public void FindNextBlueVIP()
        {
            this._nexBlueVIP = FindEligibleVip(1, false);
        }

        public bool IsNextBlueVipEligible()
        {
            if (this._nexBlueVIP == null)
                return false;
            return this._nexBlueVIP.Death;
        }

        public void SetBlueVip(RoomSessionPlayer player)
        {
            this._blueVIP = player;
            this._nexBlueVIP = null;

            if (player == null)
                this._blueVIPSetTime = uint.MaxValue;
            else
                this._blueVIPSetTime = Util.Util.Timestamp();
        }

        public uint GetElapsedBlueVipTime()
        {
            if (this._blueVIPSetTime == uint.MaxValue)
                return 0;

            return Util.Util.Timestamp() - this._blueVIPSetTime;
        }

        public void FindNextYellowVip()
        {
            this._nexYellowVIP = FindEligibleVip(2, false);
        }

        public bool IsNextYellowVipEligible()
        {
            if (this._nexYellowVIP == null)
                return false;
            return this._nexYellowVIP.Death;
        }

        public void SetYellowVip(RoomSessionPlayer player)
        {
            this._yellowVIP = player;
            this._nexYellowVIP = null;

            if (player == null)
                this._yellowVIPSetTime = uint.MaxValue;
            else
                this._yellowVIPSetTime = Util.Util.Timestamp();
        }

        public uint GetElapsedYellowVipTime()
        {
            if (this._yellowVIPSetTime == uint.MaxValue)
                return 0;

            return Util.Util.Timestamp() - this._yellowVIPSetTime;
        }

        public bool IsVip(RoomSessionPlayer player)
        {
            return player.Team == 1 && player == this._blueVIP || player.Team == 2 && player == this._yellowVIP;
        }

        public void SpawnPlayer(RoomSessionPlayer player)
        {
            player.MakeInvincible();
            player.SetHealth(player.GetDefaultHealth());
            player.WeaponManager.Reset();

            var spawn = Game.Instance.SpawnManager.GetRandomSpawn(this._room.Map, player.Team);

            if (this._room.Mode == GameMode.Mode.VIP)
            {
                // decide who s VIP at respawning
                if ((this._blueVIP == null && player.Team == 1) || (player == this._nexBlueVIP && (GetElapsedBlueVipTime() > 100) || this._blueVIP.Death))
                    SetBlueVip(player);
                else if ((this._yellowVIP == null && player.Team == 2) || (player == this._nexYellowVIP && (GetElapsedYellowVipTime() > 100) || this._yellowVIP.Death))
                    SetYellowVip(player);
            }

            RelayPlaying<GCRespawn>(player.Player.PlayerId, (uint)player.Character, (uint)0, spawn.X, spawn.Y, spawn.Z, IsVip(player));
        }

        public void SyncPlayer(RoomSessionPlayer player)
        {
            lock (player.Lock)
            {
                foreach (var p in this._players)
                {
                    if (p.Value == player)
                        continue;

                    player.Post(new GCRespawn(p.Key, p.Value.Character, 0, 255f, 255f, 255f, IsVip(player)));
                    player.Post(new GCGameState(p.Key, 8));
                    var weapon = p.Value.WeaponManager.SelectedWeapon;
                    player.Post(new GCWeapon(p.Key, 0, weapon.ItemId, 0));
                }
            }

            this._itemManager.SyncPlayer(player);
            this._gameMode.OnPlayerSync(player);
        }

        public uint GetElapsedTime()
        {
            uint currTime = Util.Util.Timestamp();

            if (currTime <= this._startTime)
                return 0;

            return (currTime = this._startTime) * 1000; // ms??
        }

        public uint GetTimeLeftInSeconds()
        {
            if (this._isPoints)
                return 1;

            var currTime = Util.Util.Timestamp();

            if (currTime > this._endTime)
                return 0;

            return this._endTime - currTime;
        }


        public void KillPlayer(RoomSessionPlayer killer, RoomSessionPlayer target, uint weaponId, bool isHeadshot)
        {
            RelayPlaying<GCGameState>(target.Player.PlayerId, isHeadshot ? (uint)28 : (uint)17, weaponId, killer.Player.PlayerId);
        }

        public RoomSessionPlayer FindEligibleVip(byte team, bool noConditions)
        {
            lock (this._lockPlayers)
            {
                List<RoomSessionPlayer> players = new List<RoomSessionPlayer>();
                int playerCount = 0;
                foreach (var p in this._players)
                {
                    if (p.Value.Spectating || p.Value.Player.Rank == 3 && this._room.EventRoom) // NOTE: rank 3 is GM
                        continue;

                    if (p.Value.Team == team && (noConditions || (p.Value.Death && p.Value != this._blueVIP && p.Value != this._yellowVIP)))
                    {
                        players.Add(p.Value);
                        playerCount++;
                    }
                }

                if (playerCount == 0)
                    return null;

                Random rnd = new Random();
                return players[rnd.Next(0, playerCount)];
            }
        }

        //
        public void Relay<T>(params object[] args)
        {
            Type[] types = args.Select(x => x.GetType()).ToArray();
            var ctor = typeof(T).GetConstructor(types);

            lock (this._lockPlayers)
                foreach (var p in this._players)
                    p.Value.Post((GameNetEvent)ctor.Invoke(args));
        }

        public void RelayExcept<T>(uint playerId, params object[] args)
        {
            Type[] types = args.Select(x => x.GetType()).ToArray();
            var ctor = typeof(T).GetConstructor(types);

            lock (this._lockPlayers)
                foreach (var p in this._players)
                    if (p.Key != playerId)
                        p.Value.Post((GameNetEvent)ctor.Invoke(args));
        }

        public void RelayPlaying<T>(params object[] args)
        {
            Type[] types = args.Select(x => x.GetType()).ToArray();
            var ctor = typeof(T).GetConstructor(types);

            lock (this._lockPlayers)
                foreach (var p in this._players)
                    if (p.Value.Playing)
                        p.Value.Post((GameNetEvent)ctor.Invoke(args));
        }

        public void RelayPlayingExcept<T>(uint playerId, params object[] args)
        {
            Type[] types = args.Select(x => x.GetType()).ToArray();
            var ctor = typeof(T).GetConstructor(types);

            lock (this._lockPlayers)
                foreach (var p in this._players)
                    if (p.Key != playerId)
                        if (p.Value.Playing)
                            p.Value.Post((GameNetEvent)ctor.Invoke(args));
        }

        public void RelayState(params object[] args)
        {
            Type[] types = args.Select(x => x.GetType()).ToArray();
            var ctor = typeof(GCGameState).GetConstructor(types);

            lock (this._lockPlayers)
                foreach (var p in this._players)
                    p.Value.Post((GCGameState)ctor.Invoke(args));
        }
    }
}
