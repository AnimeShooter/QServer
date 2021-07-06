using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Network;

namespace Qserver.GameServer.Qpang
{
    public class Room
    {
        private object _lock;
        private Dictionary<uint, RoomPlayer> _players;

        private uint _host;
        private ushort _port;

        private uint _id;
        private string _name;

        private string _password;

        private byte _map;
        private byte _mode;
        private byte _state;
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
