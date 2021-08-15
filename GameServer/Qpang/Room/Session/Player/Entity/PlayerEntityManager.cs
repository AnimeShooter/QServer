using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class PlayerEntityManager
    {
        private RoomSessionPlayer _player;
        private uint _bulletIndex;
        private PlayerBulletEntity[] _validBullets;

        public PlayerEntityManager()
        {
            
        }

        public void Initialize(RoomSessionPlayer player)
        {
            this._player = player;
            this._validBullets = new PlayerBulletEntity[20];
        }

        public void Shoot(uint entityId)
        {
            _validBullets[_bulletIndex] = new PlayerBulletEntity(entityId);
            if (_bulletIndex == 19)
                _bulletIndex = 0;
            else
                _bulletIndex++;
        }

        public bool IsValidShot(uint enitiyId)
        {
            for(int i = 0; i < 20; i++)
            {
                var entity = _validBullets[i];
                if (entity != null && entity.Id == enitiyId)
                    return true;
            }
            return false;
        }

        public void AddKill(uint entityId)
        {
            lock(_player)
            {
                if (_player == null)
                    return;

                for(int i = 0; i < 20; i++)
                {
                    var entity = _validBullets[i];
                    if(entity.Id == entityId)
                    {
                        entity.AddKill();
                        var killCount = entity.KillCount;
                        if (killCount > _player.HighestMultiKill)
                            _player.HighestMultiKill = killCount;
                        return;
                    }
                }
            }
        }

        public void Close()
        {
            lock (_player)
            {
                if (_player == null)
                    return;

                for (int i = 0; i < 20; i++)
                {
                    var entity = _validBullets[i];
                    var killCount = entity.KillCount;
                    if (killCount > _player.HighestMultiKill)
                        _player.HighestMultiKill = killCount;
                }
            }
        }
    }
}
