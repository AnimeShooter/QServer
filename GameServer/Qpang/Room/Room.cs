using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Network;

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
        private byte _mode;
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
        public byte Map
        {
            get { return this._map; }
            set { this._map = value; }
        }
        public byte Mode
        {
            get { return this._mode; }
            set { this._mode = value; }
        }
        public GameMode ModeManager
        {
            get { return this._modeManager; }
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
        public bool IsLevelLimited
        {
            get { return this._isLevelLimited; }
            set { this._isLevelLimited = value; }
        }
        public bool IsTeamSorting
        {
            get { return this._isTeamSorting; }
            set { this._isTeamSorting = value; }
        }
        public bool IsSkillsEnabled
        {
            get { return this._isSkillsEnabled; }
            set { this._isSkillsEnabled = value; }
        }
        public bool IsMeleeOnly
        {
            get { return this._isMeleeOnly; }
            set { this._isMeleeOnly = value; }
        }


        public Room(uint id, string name, byte map, byte mode, uint host, ushort port)
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

            this._modeManager = Game.Instance.RoomManager.GameModeManager.GetGameMode(mode);

            this._players = new Dictionary<uint, RoomPlayer>();

        }

        public RoomAPI ToAPI()
        {
            return new RoomAPI()
            {
                Id = this._id,
                Name = this._name,
                Password = this._password != "",
                Map =  this._map,
                Mode = ((GameModeName)this._mode).ToString(),
                PlayerCount =  this._playerCount,
                MaxPlayers = this._maxPlayers,
                LevelLimited = this._isLevelLimited,
                TeamSorting = this._isTeamSorting,
                SkillsEnabled  = this._isSkillsEnabled,
                MeleeOnly =  this._isMeleeOnly,
                ScorePoints = this._scorePoints,
                ScoreTime = this._scoreTime,
                PointsGame = this._isPointsGame,
                Started = this._isPlaying
            };
        }

        public void AddPlayer(ConnServer conn)
        {
            if (conn == null || conn.Player == null)
                return;

            //var roomPlayer = new RoomPlayer(conn, this);
            //conn.Player.RoomPlayer = roomPlayer;

            //if (this._players.Count == 0)
            //    this._masterPlayerId = roomPlayer.Player.PlayerId;

            ////roomPlayer.setteam
            //lock(this._lock)
            //    this._players.Add(conn.Player.PlayerId, roomPlayer);

            //conn.EnterRoom(this);
            //SyncPlayers(roomPlayer);

        }

        public void RemovePlayer(uint id)
        {
            lock(this._players)
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

        }

        public uint FindNewMaster()
        {
            lock(this._lock)
            {
                if (this._players.Count == 0)
                    return 0;

                // return first key xD
                foreach (var p in this._players)
                    return p.Key;
            }
            return 0;
        }
    }
}
