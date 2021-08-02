using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class CacheManager
    {
        private PlayerCacheManager _playerCacheManager;

        public PlayerCacheManager PlayerCacheManager
        {
            get { return this._playerCacheManager; }
        }

        public CacheManager()
        {
            this._playerCacheManager = new PlayerCacheManager();
        }

        public void Clear()
        {
            this._playerCacheManager.Clear();
        }
    }
}
