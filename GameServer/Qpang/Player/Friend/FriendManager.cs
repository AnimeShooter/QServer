using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class FriendManager
    {
        private object _lock;
        private Player _player;

        private ushort _incomingSlot = 10;
        private ushort _outgoingSlot = 10;
        private ushort _friendSlot = 30;

        private Dictionary<uint, Friend> _friends;
        private Dictionary<uint, Friend> _outgoingFriends;
        private Dictionary<uint, Friend> _incomingFriends;

        public FriendManager(Player player)
        {
            this._player = player;
            this._lock = new object();


            // TODO: friends database
            this._friends = new Dictionary<uint, Friend>();
            this._outgoingFriends = new Dictionary<uint, Friend>();
            this._incomingFriends = new Dictionary<uint, Friend>();
        }

        public List<Friend> List()
        {
            lock(this._lock)
            {
                var friends = new List<Friend>();
                foreach (var f in this._friends)
                    friends.Add(f.Value);
                return friends;
            }
        }
        public void AppearOnline()
        {
            //if(this._player != null)

        }

        public void AppearOffline()
        {

        }

        public void send()
        {

        }

        public void Close()
        {

        }
    }
}
