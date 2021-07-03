using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang.Room.Session.Player.Entity
{
    public class PlayerEntityManager
    {
        private RoomSessionPlayer _player;
        private uint _bulletIndex;
        private PlayerBulletEntity[] _validBullets = new PlayerBulletEntity[20];

        public void Initialize(RoomSessionPlayer player)
        {
            
        }

        public void Shoot(uint entityId)
        {

        }

        public bool IsValidShot(uint enitiyId)
        {
            return true;
        }

        public void AddKill(uint entityId)
        {

        }

        public void Close()
        {

        }
    }
}
