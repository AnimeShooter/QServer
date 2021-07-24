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

            var memos = Game.Instance.MemoRepository.GetMemos(player.PlayerId).Result;
            foreach(var m in memos)
            {
                this._memos.Add(m.id, new Memo()
                {
                    Id = m.id,
                    SenderId = m.sender_id,
                    Nickname = m.Name,
                    Message = m.message,
                    IsOpened = m.opened == 1
                });
            }

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
