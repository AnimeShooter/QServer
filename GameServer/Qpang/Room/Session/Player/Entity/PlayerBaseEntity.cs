using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class PlayerBaseEntity
    {
        private uint _id;
        private uint _killCount;
        private List<uint> _hitPlayers;

        public uint Id
        {
            get { return _id; }
        }

        public uint KillCount
        {
            get { return _killCount; }
        }

        public PlayerBaseEntity(uint id)
        {
            this._id = id;
            this._killCount = 0;
        }

        bool IsPlayerValidForHit(uint playerId)
        {
            for (int i = 0; i < this._hitPlayers.Count; i++)
            {
                if (this._hitPlayers[i] == playerId)
                    return true;
            }
            return false;
        }

        public void AddKill()
        {
            this._killCount++;
        }

        public void OnPlayerHit(uint playerId)
        {
            this._hitPlayers.Add(playerId);
        }

    }
}
