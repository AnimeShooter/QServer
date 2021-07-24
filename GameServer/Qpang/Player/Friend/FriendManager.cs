﻿using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Handlers;
using Qserver.GameServer.Network.Packets;
using Qserver.GameServer.Network.Managers;

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

            var friends = Game.Instance.FriendsRepository.GetFriends(this._player.PlayerId).Result;
            foreach(var f in friends)
            {
                var nf = new Friend()
                {
                    FriendId = f.player_to,
                    Nickname = f.Name,
                    Level = f.Level,
                    Rank = f.Rank,
                    IsOnline = f.status == 1 ? Game.Instance.GetOnlinePlayer(f.player_to) != null : false
                };
                switch(f.status)
                {
                    case 1:
                        this._friends.Add(f.id, nf);
                        break;
                    case 2:
                        this._outgoingFriends.Add(f.id, nf);
                        break;
                    case 4:
                        this._incomingFriends.Add(f.id, nf);
                        break;
                }
            }

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
            lock (this._lock)
                if (this._player != null)
                    send(LobbyManager.Instance.AppearOnline());
        }

        public void AppearOffline()
        {
            lock (this._lock)
                if (this._player != null)
                    send(LobbyManager.Instance.AppearOffline());
        }

        public void send(PacketWriter packet)
        {
            lock(this._lock)
            {
                foreach (var f in this._friends)
                {
                    var player = Game.Instance.GetOnlinePlayer(f.Key);
                    if (player != null)
                        player.SendLobby(packet);
                }     
            }    
        }

        public void Close()
        {

        }

        public bool HasIncomingSlot()
        {

        }

        public bool HasOutgoingSlot()
        {

        }

        public bool HasFriendSlot()
        {

        }

        public bool Contains(uint playerId)
        {
            lock(this._lock)
                return this._friends.ContainsKey(playerId) || this._incomingFriends.ContainsKey(playerId) || this._outgoingFriends.ContainsKey(playerId);
        }

        public void AddIncomingFriend(Player target)
        {
            if (target == null)
                return;

            lock(this._lock)
            {
                Game.Instance.FriendsRepository.AddFriend(this._player.PlayerId, target.PlayerId, 4).GetAwaiter().GetResult();
                var newFriend = new Friend()
                {
                    FriendId = target.PlayerId,
                    Nickname = target.Name,
                    Level = target.Level,
                    Rank = target.Rank
                };
                this._incomingFriends.Add(target.PlayerId, newFriend);
                this._player.SendLobby(LobbyManager.Instance.AddIncommingFriend(newFriend));
            }    
        }

        public void AddOutgoingFriend(Player target)
        {
            if (target == null)
                return;

            lock (this._lock)
            {
                Game.Instance.FriendsRepository.AddFriend(this._player.PlayerId, target.PlayerId, 2).GetAwaiter().GetResult();
                var newFriend = new Friend()
                {
                    FriendId = target.PlayerId,
                    Nickname = target.Name,
                    Level = target.Level,
                    Rank = target.Rank
                };
                this._outgoingFriends.Add(target.PlayerId, newFriend);
                this._player.SendLobby(LobbyManager.Instance.AddOutgoingFriend(newFriend));
            }
        }

        public void RemoveOutgoing(uint friendId)
        {
            lock(this._lock)
            {
                if (!this._outgoingFriends.ContainsKey(friendId))
                    return;

                this._outgoingFriends.Remove(friendId);
                Game.Instance.FriendsRepository.RemoveFriend(this._player.PlayerId, friendId).GetAwaiter().GetResult();
            }
        }

        public void RemoveIncoming(uint friendId)
        {
            lock (this._lock)
            {
                if (!this._incomingFriends.ContainsKey(friendId))
                    return;

                this._incomingFriends.Remove(friendId);
                Game.Instance.FriendsRepository.RemoveFriend(this._player.PlayerId, friendId).GetAwaiter().GetResult();
            }
        }

        public void AcceptIncoming(Player target)
        {
            if (target == null)
                return;

            lock(this._lock)
            {
                if (!this._incomingFriends.ContainsKey(target.PlayerId))
                    return;

                var friend = this._incomingFriends[target.PlayerId];
                friend.State = 1;
                friend.IsOnline = target.Online;
                this._friends.Add(friend.FriendId, friend);

                Game.Instance.FriendsRepository.UpdateFriendState(this._player.PlayerId, target.PlayerId, 1).GetAwaiter().GetResult();
                this._player.SendLobby(LobbyManager.Instance.AcceptIncommingFriend(friend));

                this._incomingFriends.Remove(target.PlayerId);
            }
        }

        public void OnOutgoingAccepted(Player target)
        {
            if (target == null)
                return;

            lock (this._lock)
            {
                if (!this._outgoingFriends.ContainsKey(target.PlayerId))
                    return;

                var friend = this._outgoingFriends[target.PlayerId];
                friend.State = 1;
                friend.IsOnline = target.Online;
                this._friends.Add(friend.FriendId, friend);

                Game.Instance.FriendsRepository.UpdateFriendState(this._player.PlayerId, target.PlayerId, 1).GetAwaiter().GetResult();
                this._player.SendLobby(LobbyManager.Instance.OutgoingFriendAccepted(friend));

                this._outgoingFriends.Remove(target.PlayerId);
            }
        }
        public void Remove(uint friendId)
        {
            if (this._player == null)
                return;

            lock(this._lock)
            {
                if (!this._friends.ContainsKey(friendId))
                    return;

                this._friends.Remove(friendId);
                
                this._player.SendLobby(LobbyManager.Instance.RemoveFriend(friendId));
                Game.Instance.FriendsRepository.RemoveFriend(this._player.PlayerId, friendId).GetAwaiter().GetResult();
            }
        }
        public void OnRemove(uint friendId)
        {
            if (this._player == null)
                return;

            lock (this._lock)
            {
                if (!this._friends.ContainsKey(friendId))
                    return;
                
                this._player.SendLobby(LobbyManager.Instance.FriendRemoved(this._friends[friendId]));
                this._friends.Remove(friendId);
                Game.Instance.FriendsRepository.RemoveFriend(this._player.PlayerId, friendId).GetAwaiter().GetResult();
            }
        }
    }

}
