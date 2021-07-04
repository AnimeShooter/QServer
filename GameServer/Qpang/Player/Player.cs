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
        private MemoManager _memoManager;
        private StatsManager _statsManager;
        private AchievementContainer _achievementContainer;

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

        public uint PlayerId
        {
            get { return this._playerId; }
        }
        public Player(uint playerId)
        {
            this._lock = new object();
            this._playerId = playerId;
            this._loginTime = DateTime.UtcNow;

            // Load player WHERE id = X

            this._inventoryManager = new InventoryManager(this);
            this._equipmentManager = new EquipmentManager(this);
            this._friendManager = new FriendManager(this);
            this._memoManager = new MemoManager(this);
            this._statsManager = new StatsManager(this);
            this._achievementContainer = new AchievementContainer(playerId);
            this._isOnline = true;
        
        }
    }
}
