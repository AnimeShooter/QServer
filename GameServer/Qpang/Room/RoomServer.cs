using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Network;
using Qserver.Util;

namespace Qserver.GameServer.Qpang
{
    public class RoomServer
    {
        private object _lockConn;
        private List<uint> _connsToDispose;
        private Dictionary<uint, ConnServer> _connections;
        private GameNetInterface _gameNetInterface;
        private DateTime _lastDisposal;
        private DateTime _lastTick;

        public RoomServer()
        {
            this._lockConn = new object();
            var port = Settings.SERVER_PORT_ROOM;
            this._gameNetInterface = new GameNetInterface();
            this._connections = new Dictionary<uint, ConnServer>();
            this._connsToDispose = new List<uint>();
        }

        public void HandleEvent(GameNetEvent e)
        {

        }

        public void Tick()
        {
            // TODO
        }

        public void CreateConnection(uint playerId, ConnServer conn)
        {
            lock(this._lockConn)
            {
                var player = Game.Instance.GetOnlinePlayer(playerId);
                if (player == null)
                    return;

                conn.Player = player;
                this._connections.Add(playerId, conn);

                bool alreadyqueuedDisposal = this._connsToDispose.Contains(playerId);
                if (alreadyqueuedDisposal)
                    return;

                this._connsToDispose.Add(playerId);
            }
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
