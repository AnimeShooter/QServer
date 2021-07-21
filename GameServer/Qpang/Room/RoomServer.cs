using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Qserver.GameServer.Network;
using Qserver.Util;

namespace Qserver.GameServer.Qpang
{
    public class RoomServer
    {
        private object _lockConn;
        private bool _isRunning;
        private List<uint> _connsToDispose;
        private Dictionary<uint, GameConnection> _connections;
        private GameNetInterface _gameNetInterface;
        private uint _lastDisposal;
        private uint _lastTick;

        public RoomServer()
        {
            this._lockConn = new object();
           
            this._gameNetInterface = new GameNetInterface();
            this._connections = new Dictionary<uint, GameConnection>();
            this._connsToDispose = new List<uint>();
            this._isRunning = false;
        }

        public void HandleEvent(GameNetEvent e)
        {
            ProcessEvent(e);
        }

        public void Initialize()
        {
            this._lastDisposal = Util.Util.Timestamp();
            this._gameNetInterface = new GameNetInterface(new IPEndPoint(0x7F000001, Settings.SERVER_PORT_ROOM));


        }
        public void Run()
        {
            // TODO
        }
        public void Tick()
        {
            // TODO
        }

        public bool CreateConnection(uint playerId, GameConnection conn)
        {
            lock(this._lockConn)
            {
                var player = Game.Instance.GetOnlinePlayer(playerId);
                if (player == null)
                    return false;

                conn.Player = player;
                this._connections.Add(playerId, conn);

                bool alreadyqueuedDisposal = this._connsToDispose.Contains(playerId);
                if (alreadyqueuedDisposal)
                    return false;

                this._connsToDispose.Add(playerId);
            }
            return true;
        }

        public void ProcessEvent(GameNetEvent e)
        {
            try
            {
                bool AuthEvent = e.Id == GameNetId.CG_AUTH;
                bool Authorized = e.GameConnection.Player != null;

                if (Authorized && !AuthEvent)
                    e.Handle(e.GameConnection, e.GameConnection.Player);
                else if(!Authorized && AuthEvent)
                    e.Handle(e.GameConnection, e.GameConnection.Player);
            }catch(Exception ex)
            {
                Log.Message(LogType.ERROR, ex.ToString());
            }
        }

        public void DropConnection(uint playerId)
        {
            lock (this._lockConn)
            {
                if (!this._connections.ContainsKey(playerId))
                    return;

                var conn = this._connections[playerId];
            }
        }
    }
}
