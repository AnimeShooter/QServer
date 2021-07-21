using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class RoomPlayer
    {
        private byte _team;
        private bool _isReady;
        private bool _isPlaying;
        private bool _isSpectating;
        private GameConnection _conn;
        private Room _room;
        private RoomSessionPlayer _roomSessionPlayer;

        public byte Team
        {
            get { return this._team; }
        }
        public bool Ready
        {
            get { return this._isReady; }
            set { this._isReady = value; }
        }
        public bool Playing
        {
            get { return this._isPlaying; }
            set { this._isPlaying = value; }
        }
        public bool Spectating
        {
            get { return this._isSpectating; }
            set { this._isSpectating = value; }
        }
        public GameConnection Conn
        {
            get { return this._conn; }
        }
        public Room Room
        {
            get { return this._room; }
        }
        public RoomSessionPlayer RoomSessionPlayer
        {
            get { return this._roomSessionPlayer; }
            set { this._roomSessionPlayer = value; }
        }

        public RoomPlayer(GameConnection conn, Room room)
        {
            this._conn = conn;
            this._room = room;
            this._team = 0;
            this._isReady = false;
            this._isPlaying = false;
            this._isSpectating = false;
        }

        public void Send(GameNetEvent e)
        {
            this._conn.PostNetEvent(e); // TODO
        }

        public void OnStart()
        {
            this._conn.Player.EquipmentManager.Save();
        }

        public void SetTeam(byte team)
        {
            this._team = team;
            //this._room.BroadcastWaiting<GCPlayerChange>(this._conn.Player, 2, team);
        }

        public void SetReady(bool ready)
        {
            this._isReady = ready;
            //this._room.BroadcastWaiting<GCReady>(this._conn.Player.PlayerId, ready);
        }
    }
}
