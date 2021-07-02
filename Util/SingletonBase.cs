using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Singleton
{
    public abstract class SingletonBase<T> where T : SingletonBase<T>
    {
        public static T Instance
        {
            get
            {
                return Singleton.GetInstance<T>();
            }
        }
    }
}
