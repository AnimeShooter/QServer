using System;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;
using System.Text;
using Qserver.GameServer.Network;
using Qserver.Util;
using TNL.Entities;
using System.Threading;

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
            this._connections = new Dictionary<uint, GameConnection>();
            this._connsToDispose = new List<uint>();
            this._isRunning = false;
            this._lastDisposal = Util.Util.Timestamp();
            this._gameNetInterface = new GameNetInterface(Settings.SERVER_PORT_ROOM); // No one cares about IP?
        }

        public void HandleEvent(GameNetEvent e)
        {
            ProcessEvent(e);
        }

        public void Run()
        {
            this._isRunning = true;
            while(this._isRunning)
            {
                lock(this._lockConn)
                {
                    try
                    {
                        this._gameNetInterface.ProcessConnections();
                        this._gameNetInterface.CheckIncomingPackets();
                        Tick();
                        Thread.Sleep(1); // dont fry the CPU
                    }catch(Exception ex)
                    {
                        Log.Message(LogType.ERROR, ex.ToString());
                    }
                }
            }
        }
        public void Tick()
        {
            var currTime = Util.Util.Timestamp();
            if(this._lastTick < currTime)
            {
                this._lastTick = currTime;
                Game.Instance.RoomManager.Tick();
            }

            if (this._connsToDispose.Count == 0)
                return;

            if(this._lastDisposal <= currTime -1)
            {
                this._lastDisposal = currTime;
                lock(this._lockConn)
                {
                    foreach(var id in this._connsToDispose)
                    {
                        if (!this._connections.ContainsKey(id))
                            continue;

                        var conn = this._connections[id];
                        if(conn != null)
                        {
                            var roomPlayer = conn.Player.RoomPlayer;
                            if (roomPlayer != null)
                                roomPlayer.Room.RemovePlayer(id);

                            if (conn.GetConnectionState() == NetConnectionState.Connected)
                                conn.Disconnect("disconnected by server");
                        }
                        this._connections.Remove(id);
                    }
                    this._connections.Clear();
                }
            }
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
