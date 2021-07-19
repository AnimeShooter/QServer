using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class GameConnection
    {
        private Player _player;

        public Player Player
        {
            get { return this._player; }
            set { this._player = value; }
        }

        public GameConnection()
        {
            // TODO: ase TNL.EventConnection
        }

        public void PostNetEvent(GameNetEvent e)
        {
            // TODO
        }

    }
}
