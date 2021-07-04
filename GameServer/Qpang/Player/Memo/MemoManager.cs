using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class MemoManager
    {
        private Player _player;
        private Dictionary<uint, Memo> _memos;

        private object _lock;
        public MemoManager(Player player)
        {
            this._player = player;
            this._lock = new object();

            this._memos = new Dictionary<uint, Memo>();
            //this._memos.Add(0, new Memo()
            //{
                
            //})
            // TODO: memo SQL

        }

        public List<Memo> List()
        {
            lock(this._lock)
            {
                List<Memo> result = new List<Memo>();
                foreach(var m in this._memos)
                {
                    result.Add(m.Value);
                }
                return result;
            }
        }
    }
}
