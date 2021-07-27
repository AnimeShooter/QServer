using System.Collections.Generic;

namespace Qserver.GameServer.Qpang
{
    public struct RoomAPI
    {
        public uint Id;
        public string Name;
        public bool Password;

        public byte Map;
        public string Mode;
        public byte PlayerCount;
        public byte MaxPlayers;
        public bool LevelLimited;
        public bool TeamSorting;
        public bool SkillsEnabled;
        public bool MeleeOnly;

        public uint ScorePoints;
        public uint ScoreTime;
        public bool PointsGame;
        public bool Started;
    }

    public class Room
    {
        private object _lock;
        private Dictionary<uint, RoomPlayer> _players;

        private uint _host;
        private ushort _port;

        private uint _id;
        private string _name;

        private string _password = "";

        private byte _map;
        private GameMode.Mode _mode;
        private byte _state;
        private byte _playerCount;
        private byte _maxPlayers;
        private uint _masterPlayerId;
        private bool _isLevelLimited;
        private bool _isTeamSorting;
        private bool _isSkillsEnabled;
        private bool _isMeleeOnly;

        private uint _scorePoints;
        private uint _scoreTime;
        private bool _isPointsGame;
        private bool _isPlaying;
        private bool _isEventRoom;

        private GameMode _modeManager;
        private RoomSession _roomSession;

        public uint Host
        {
            get { return this._host; }
        }
        public ushort Port
        {
            get { return this._port; }
        }
        public uint Id
        {
            get { return this._id; }
        }
        public string Name
        {
            get { return this._name; }
        }
        public string Password
        {
            get { return this._password; }
        }
        public byte Map
        {
            get { return this._map; }
            set { this._map = value; }
        }
        public GameMode.Mode Mode
        {
            get { return this._mode; }
            set { this._mode = value; }
        }
        public GameMode ModeManager
        {
            get { return this._modeManager; }
        }
        public RoomSession RoomSession 
        {
            get { return this._roomSession; }
        }
        public byte State
        {
            get { return this._state; }
            set { this._state = value; }
        }
        public byte PlayerCount
        {
            get { return this._playerCount; }
            set { this._playerCount = value; }
        }

        public byte MaxPlayers
        {
            get { return this._maxPlayers; }
            set { this._maxPlayers = value; }
        }
        public bool LevelLimited
        {
            get { return this._isLevelLimited; }
            set { this._isLevelLimited = value; }
        }
        public bool TeamSorting
        {
            get { return this._isTeamSorting; }
            set { this._isTeamSorting = value; }
        }
        public bool SkillsEnabled
        {
            get { return this._isSkillsEnabled; }
            set { this._isSkillsEnabled = value; }
        }
        public bool MeleeOnly
        {
            get { return this._isMeleeOnly; }
            set { this._isMeleeOnly = value; }
        }

        public bool PointsGame
        {
            get { return this._isPointsGame; }
        }
        public bool Playing
        {
            get { return this._isPlaying; }
        }
        public bool EventRoom
        {
            get { return this._isEventRoom; }
            set { this._isEventRoom = value; }
        }

        public uint ScorePoints
        {
            get { return this._scorePoints; }
        }
        public uint ScoreTime
        {
            get { return this._scoreTime; }
        }

        public uint MasterId
        {
            get { return this._masterPlayerId; }
        }

        public Room(uint id, string name, byte map, GameMode.Mode mode, uint host, ushort port)
        {
            this._lock = new object();
            this._id = id;
            this._name = name;
            this._map = map;
            this._mode = mode;
            this._host = host;
            this._port = port;

            this._state = 2;
            this._maxPlayers = 16;
            this._isLevelLimited = false;
            this._isTeamSorting = false;
            this._isSkillsEnabled = false;
            this._isMeleeOnly = false;
            this._isPointsGame = false;
            this._scorePoints = 40;
            this._scoreTime = 10;
            this._isPlaying = false;

            this._modeManager = Game.Instance.RoomManager.GameModeManager.Get(mode);

            this._players = new Dictionary<uint, RoomPlayer>();

        }

        public RoomAPI ToAPI()
        {
            return new RoomAPI()
            {
                Id = this._id,
                Name = this._name,
                Password = this._password != "",
                Map = this._map,
                Mode = ((GameModeName)this._mode).ToString(),
                PlayerCount = this._playerCount,
                MaxPlayers = this._maxPlayers,
                LevelLimited = this._isLevelLimited,
                TeamSorting = this._isTeamSorting,
                SkillsEnabled = this._isSkillsEnabled,
                MeleeOnly = this._isMeleeOnly,
                ScorePoints = this._scorePoints,
                ScoreTime = this._scoreTime,
                PointsGame = this._isPointsGame,
                Started = this._isPlaying
            };
        }

        public void AddPlayer(GameConnection conn)
        {
            if (conn == null || conn.Player == null)
                return;

            var roomPlayer = new RoomPlayer(conn, this);
            conn.Player.RoomPlayer = roomPlayer;

            if (this._players.Count == 0)
                this._masterPlayerId = roomPlayer.Player.PlayerId;

            roomPlayer.SetTeam(GetAvailableTeam());
            lock (this._lock)
                this._players.Add(conn.Player.PlayerId, roomPlayer);

            conn.EnterRoom(this);
            //SyncPlayers(roomPlayer);

        }

        public void RemovePlayer(uint id)
        {
            lock (this._players)
            {
                if (this._players.ContainsKey(id))
                    this._players.Remove(id);

                if (id == this._masterPlayerId)
                    this._masterPlayerId = FindNewMaster();

                if (this._roomSession != null)
                    this._roomSession.RemovePlayer(id);

                //BroadcastWaiting
            }
        }

        public void Tick()
        {
            if (this._isPlaying && this._roomSession != null)
                this._roomSession.Tick();
        }

        public void Start()
        {
            if (this._isPlaying)
                return;

            this._roomSession = new RoomSession(this, this._modeManager);
            this._roomSession.Initialize(); // TODO

            this._isPlaying = true;
            this._state = 64;

            lock (this._lock)
            {
                foreach (var p in this._players)
                {
                    if (p.Value.Ready || this._masterPlayerId == p.Key)
                    {
                        p.Value.SetReady(true);
                        p.Value.Playing = true;
                        p.Value.Conn.StartLoading(this, p.Value);
                        p.Value.OnStart();
                    }
                    else
                        p.Value.Conn.StartGameButNotReady();
                }
            }
        }

        public void SyncPlayers(RoomPlayer player)
        {
            lock(this._lock)
            {
                player.Conn.PostNetEvent(new GCJoin(player));
                foreach(var  p in this._players)
                {
                    if(player != p.Value)
                    {
                        if (!p.Value.Playing)
                            p.Value.Conn.PostNetEvent(new GCJoin(player));
                        p.Value.Conn.PostNetEvent(new GCJoin(p.Value));
                    }
                }
            }
        }

        public void Close()
        {
            lock (this._lock)
            {
                foreach (var p in this._players)
                    p.Value.Conn.Disconnect("Room closed");

                Game.Instance.RoomManager.Remove(this._id);
            }
        }

        public void Update(uint cmd, uint val)
        {
            lock (this._lock)
            {
                foreach (var p in this._players)
                    if (!p.Value.Playing)
                        p.Value.Conn.PostNetEvent(new GCRoom(p.Key, cmd, this));
            }
        }

        public void Finish()
        {
            this._roomSession.Clear();
            this._roomSession = null;

            this._state = 2;
            this._isPlaying = false;

            UnreadyAll();
        }

        public void UnreadyAll(bool notify = false)
        {
            lock (this._lock)
            {
                foreach (var p in this._players)
                {
                    if (p.Value.Ready)
                    {
                        p.Value.SetReady(false);
                        if (notify)
                            p.Value.Player.Broadcast("Your ready status has been remove because the room rules got updated.");
                    }

                    if (p.Value.Ready)
                    {
                        p.Value.Playing = false;
                        p.Value.Spectating = false;
                    }
                }
            }
        }

        public bool CanStartInTeam(byte team)
        {
            if (this._roomSession == null)
                return true;

            if (!this._roomSession.GameMode.IsTeamMode())
                return true;

            var bluePlayerCount = this._roomSession.GetPlayersForTeam(1);
            var yellowPlayerCount = this._roomSession.GetPlayersForTeam(2);

            if (team == 1)
            {
                if (bluePlayerCount.Count == 0)
                    return true;

                if (bluePlayerCount.Count - 1 >= yellowPlayerCount.Count)
                    return false;
            }
            else
            {
                if (yellowPlayerCount.Count == 0)
                    return true;

                if (yellowPlayerCount.Count - 1 >= bluePlayerCount.Count)
                    return false;
            }

            return true;
        }

        public RoomPlayer GetPlayer(uint id)
        {
            lock(this._lock)
                if (this._players.ContainsKey(id))
                    return this._players[id];    

            return null;
        }

        public uint FindNewMaster()
        {
            lock (this._lock)
            {
                if (this._players.Count == 0)
                    return 0;

                // return first key xD
                foreach (var p in this._players)
                    return p.Key;
            }
            return 0;
        }

        public void SetMode(GameMode.Mode mode)
        {
            this._mode = mode;
            this._modeManager = Game.Instance.RoomManager.GameModeManager.Get(mode);
            this._modeManager.OnApply(this);

            UnreadyAll(true);
            Update((uint)CGRoom.Command.MODE_ROOM, (uint)mode);

            // this resets after chnage of mode

            if (this._isPointsGame)
                Update((uint)CGRoom.Command.SET_POINTS, this._scorePoints);
            else
                Update((uint)CGRoom.Command.SET_TIME, this._scoreTime);

        }

        public void SetMap(byte map)
        {
            if (map > 12)
                return;

            this._map = map;
            Update((uint)CGRoom.Command.MAP_ROOM, map);
        }

        public void SetMaxPlayers(byte maxPlayers)
        {
            this._maxPlayers = maxPlayers;
            Update((uint)CGRoom.Command.PLAYERS_ROOM, maxPlayers);
        }
        public void SetScorePoints(uint points)
        {
            this._scorePoints = points;
            Update((uint)CGRoom.Command.SET_POINTS, points);
        }
        public void SetScoreTime(uint time)
        {
            this._scoreTime = time;
            Update((uint)CGRoom.Command.SET_TIME, time);
        }
        public void SetPassword(string password)
        {
            if (password.Length > 4)
                password = password.Substring(0, 4);

            this._password = password;
            Update((uint)CGRoom.Command.PASS_ROOM, (uint)0);
        }
        public void SetLevelLimited(bool levelLimited)
        {
            this._isLevelLimited = levelLimited;
            Update((uint)CGRoom.Command.LEVEL_ROOM, levelLimited ? (uint)1 : (uint)0);
        }
        public void SetTeamSorting(bool teamSorting)
        {
            this._isTeamSorting = teamSorting;
            Update((uint)CGRoom.Command.TEAM_ROOM, teamSorting ? (uint)1 : (uint)0);
        }
        public void SetSkillsEnabled(bool skillEnabled)
        {
            this._isSkillsEnabled = skillEnabled;
            Update((uint)CGRoom.Command.TOGGLE_SKILL, skillEnabled ? (uint)1 : (uint)0);
        }
        public void SetMeleeOnly(bool meleeOnly)
        {
            this._isMeleeOnly = meleeOnly;
            Update((uint)CGRoom.Command.TOGGLE_SKILL, meleeOnly ? (uint)1 : (uint)0);
        }


        public void BalancePlayers()
        {
            lock (this._lock)
            {
                foreach (var p in this._players)
                {
                    if (!this._modeManager.IsTeamMode())
                        p.Value.SetTeam(0);
                    else
                    {
                        if (p.Value.Team != 0)
                            continue;
                        p.Value.SetTeam(GetAvailableTeam());
                    }
                }
            }
        }

        public byte GetAvailableTeam()
        {
            if (!this._modeManager.IsTeamMode())
                return 0;

            int yellowCount = 0;

            lock (this._lock)
                foreach (var p in this._players)
                    if (p.Value.Team == 2)
                        yellowCount++;

            return yellowCount * 2 >= this._players.Count ? (byte)1 : (byte)2;
        }

        public bool IsTeamAvailable(byte team)
        {
            
            if (team != 1 && team != 2)
                return false;

            int teamCount = 0;
            lock (this._lock)
                foreach (var p in this._players)
                    if (p.Value.Team == team)
                        teamCount++;

            return teamCount * 2 < this._maxPlayers;
        }



        //
        public void Broadcast<T>(T e)  // TODO
        {
            lock(this._lock)
            {
                //foreach (var p in this._players)
                //    p.Value.Conn.PostNetEvent(new T());
            }
        }


        public void BroadcastWaiting<T>(T e) // TODO
        {
            lock(this._lock)
            {

            }
        }
    }
}
