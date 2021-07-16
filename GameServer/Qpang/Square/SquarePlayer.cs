using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Managers;

namespace Qserver.GameServer.Qpang
{
    public class SquarePlayer
    {
        private Player _player;
        private Square _square;

        private uint _selectedWeapon;
        private uint _state;
        private byte _stateValue;
        private float[] _position;

        public Player Player
        {
            get { return this._player; }
        }
        public Square Square
        {
            get { return this._square; }
        }
        public uint State
        {
            get { return this._state; }
        }
        public byte StateValue
        {
            get { return this._stateValue; }
        }
        public float[] Position
        {
            get { return this._position; }
        } 
        public uint SelectedWeapon
        {
            get { return this._selectedWeapon; }
        }

        public SquarePlayer(Player player, Square square)
        {
            this._square = square;
            this._player = player;
            this._position = new float[3];
            this._state = 1;
            this._stateValue = 0;
            this._selectedWeapon = player.EquipmentManager.GetDefaultWeapon();
        }

        public void SetState(uint state, byte stateValue)
        {
            this._state = state;
            this._stateValue = stateValue;

            this._square.SendPacket(Network.SquareManager.Instance.UpdatePlayerState(this, this._stateValue));
        }

        public void SetState(uint state)
        {
            this._state = state;
            this._square.SendPacket(Network.SquareManager.Instance.UpdatePlayerState(this, this._stateValue));
        }

        public void ChangeWeapon(uint itemId)
        {
            this._selectedWeapon = itemId;
            this._square.SendPacket(Network.SquareManager.Instance.UpdatePlayerEquipment(this));

        }

        public void Move(float[] position, byte direction, byte moveType)
        {
            // TODO: add serverside calculation and check (anti TP, anit speedhack)
            this._position = position;
            this._square.SendPacket(Network.SquareManager.Instance.MovePlayer(this._player.PlayerId, position, moveType, direction));
        }

        public void Chat(string message)
        {
            this._square.SendPacket(Network.SquareManager.Instance.Chat(this._player.Name, message));
        }

        public void Emote(uint emoteId)
        {
            this._square.SendPacketExcept(Network.SquareManager.Instance.Emote(this._player.PlayerId, emoteId), this._player.PlayerId);
        }

    }
}
