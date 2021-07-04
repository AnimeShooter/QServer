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
