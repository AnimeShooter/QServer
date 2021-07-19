using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Network;

namespace Qserver.GameServer.Qpang
{
    public class RoomServer
    {
        private object _lock;
        private List<uint> _connsToDispose;
        private Dictionary<uint, ConnServer> _connections;
        private GameNetInterface _gameNetInterface;
        private DateTime _lastDisposal;
        private DateTime _lastTick;

        public RoomServer()
        {
            var port = Settings.SERVER_PORT_ROOM;
            this._gameNetInterface = new GameNetInterface();
        }

        public void HandleEvent(GameNetEvent e)
        {

        }

        //public bool CreateConnection(uint playerId, ConnServer conn)
        //{

        //}

        public void DropConnection(GameNetEvent e)
        {

        }



    }
}
