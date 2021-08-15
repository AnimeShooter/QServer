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
            this._validBullets = new PlayerBulletEntity[20];
        }

        public void Initialize(RoomSessionPlayer player)
        {
            this._player = player;
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
            if (this._player == null)
                return;

            lock (this._player.Lock)
            {
                for(int i = 0; i < 20; i++)
                {
                    var entity = _validBullets[i];
                    if (entity == null)
                        continue; // TODO fiz??
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
            if (this._player == null)
                return;

            lock (this._player.Lock)
            {
                for (int i = 0; i < 20; i++)
                {
                    var entity = _validBullets[i];
                    if (entity == null)
                        return; // ?? TODO
                    var killCount = entity.KillCount;
                    if (killCount > _player.HighestMultiKill)
                        _player.HighestMultiKill = killCount;
                }
            }
        }
    }
}
