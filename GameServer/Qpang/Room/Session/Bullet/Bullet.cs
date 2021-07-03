using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class Bullet
    {
        public uint Id;
        public uint ItemId;
        public byte KillCount;
        public byte HitIndex;
        public TimeSpan ShootTime;
        public uint[] Hits = new uint[16];

        public void AddHit(uint playerId)
        {
            Hits[HitIndex] = playerId;

            if (HitIndex < 15)
                HitIndex++;
        }

        public bool HasHit(uint playerId)
        {
            for(int i = 0; i < Hits.Length; i++)
            {
                if (Hits[i] == playerId)
                    return true;
            }
            return false;
        }
    }
}
