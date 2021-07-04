using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class Player
    {
        private InventoryManager _inventoryManager;
        private EquipmentManager _equipmentManager;
        private FriendManager _friendManager;
        private MemoManager _mamoManager;
        private StatsManager _statsManager;
        private AchievementManager _achievementManager;

        private uint _playerId;
        private uint _userId;
        private string _name;
        private byte _rank;
        private byte _prestige;
        private ushort _character;
        private uint _don;
        private uint _cash;
        private uint _coins;
        private uint _playTime;
        private DateTime _loginTime;
        private uint slackerPoints;
        private bool _isMuted;
        private bool _exists;
        private bool _isClosed;
        private bool _isOnline;

        private uint _currentSquareId;

        //private _lobbyConn; 
        //private _squareConn;

        // _squarePlayer
        // _roomPlayer

        private object _lock;

        public Player()
        {
            this._lock = new object();
        }
    }
}
