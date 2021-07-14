using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Packets;

namespace Qserver.GameServer.Qpang
{
    public class SquareManager
    {
        private object _lock;
        private Dictionary<uint, Square> _squares;
        public SquareManager()
        {
            this._lock = new object();
            this._squares = new Dictionary<uint, Square>();
            Create("TEST_SQUARE");
        }

        public Square Create(string name)
        {
            uint id = GetAvailableSquareId();
            var square = new Square(id, name);

            lock(this._lock)
            {
                this._squares.Add(id, square);
            }

            return square;
        }

        public Square Get(uint id)
        {
            lock(this._lock)
            {
                if (this._squares.ContainsKey(id))
                    return this._squares[id];
                return null;
            }
        }

        public Square GetAvailableSquare()
        {
            lock(this._lock)
            {
                foreach(var square in this._squares)
                {
                    if (square.Value.PlayerCount < square.Value.Capacity / 2)
                        return square.Value;
                }
            }
            return null;
        }

        public void Broadcast(PacketWriter packet)
        {
            lock(this._lock)
            {
                foreach (var square in this._squares)
                    square.Value.SendPacket(packet);
            }
        }

        public void Close(uint id)
        {
            if (this._squares.Count == 1)
                return; // last man standing

            // TODO
            lock(this._lock)
            {
                //foreach (var square in _squares)
                //    square.Value.SendPacket(Network.SquareManager.Instance.DeleteSquareEntry(id));
                this._squares.Remove(id);
            }
        }

        public List<Square> List()
        {
            lock(this._lock)
            {
                List<Square> squares = new List<Square>();
                foreach (var square in this._squares)
                    squares.Add(square.Value);

                return squares;
            }
        }

        public uint GetAvailableSquareId()
        {
            uint id = 1;

            lock (this._lock)
            {
                foreach(var s in this._squares)
                {
                    if (s.Key > id)
                        id = s.Key;
                }
            }
            id++;

            return id;
        }
    }
}
