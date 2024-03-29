﻿using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Packets;

namespace Qserver.GameServer.Qpang
{
    public class Square
    {
        private uint _id;
        private string _name;
        private byte _capacity;
        private byte _state;
        private bool _isClosed;

        private object _lock;

        private Dictionary<uint, SquarePlayer> _players;

        public uint Id
        {
            get { return this._id; }
        }
        public string Name
        {
            get { return this._name; }
        }
        public byte Capacity
        {
            get { return this._capacity; }
        }
        public byte PlayerCount
        {
            get { return (byte)this._players.Count; }
        }
        public byte State
        {
            get { return this._state; }
        }
        public bool IsFull
        {
            get { return this._capacity == PlayerCount; }
        }
        public bool IsClosed
        {
            get { return this._isClosed; }
        }

        public Square(uint id, string name, byte capacity = 50)
        {
            this._lock = new object();
            this._id = id;
            this._capacity = capacity;
            this._name = name;
            this._state = 8;
            this._isClosed = false;
            this._players = new Dictionary<uint, SquarePlayer>();
        }

        public bool Add(Player player)
        {
            if (player == null)
                return false;

            if (PlayerCount > this._capacity)
                return false;

            if (player.Rank == 1 && this._isClosed)
                return false;

            var squarePlayer = new SquarePlayer(player, this);
            lock (this._lock)
            {
                this._players[player.PlayerId] = squarePlayer;
            }

            Game.Instance.SquareManager.Broadcast(Network.SquareManager.Instance.UpdateSquareEntry(this, true));

            player.EnterSquare(squarePlayer);
            SendPacketExcept(Network.SquareManager.Instance.AddPlayer(squarePlayer), player.PlayerId);

            return true;
        }

        public void Remove(uint playerId)
        {
            lock (this._lock)
            {
                this._players.Remove(playerId);
            }

            SendPacket(Network.SquareManager.Instance.RemovePlayer(playerId));
            Game.Instance.SquareManager.Broadcast(Network.SquareManager.Instance.UpdateSquareEntry(this, true));

            if (this._players.Count == 0)
                Game.Instance.SquareManager.Close(this._id);
        }

        public void SendPacket(PacketWriter packet)
        {
            lock(this._lock)
            {
                foreach (var squarePlayer in this._players)
                        squarePlayer.Value.Player.SendSquare(packet);
            }
        }

        public void SendPacketExcept(PacketWriter packet, uint playerId)
        {
            lock (this._lock)
            {
                foreach (var squarePlayer in this._players)
                    if (squarePlayer.Key != playerId)
                        squarePlayer.Value.Player.SendSquare(packet);
            }
        }

        public List<SquarePlayer> ListPlayers()
        {
            lock(this._lock)
            {
                List<SquarePlayer> players = new List<SquarePlayer>();
                foreach (var p in this._players)
                    players.Add(p.Value);
                return players;
            }
        }
    }
}
