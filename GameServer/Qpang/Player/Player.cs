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
        private uint _experience;
        private uint _level;
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

        private SquarePlayer _squarePlayer;
        private RoomPlayer _roomPlayer;
        //private _squareConn;

        // _squarePlayer
        // _roomPlayer

        private object _lock;

        public InventoryManager InventoryManager
        {
            get { return this._inventoryManager; }
        }
        public EquipmentManager EquipmentManager
        {
            get { return this._equipmentManager; }
        }
        public FriendManager FriendManager
        {
            get { return this._friendManager; }
        }
        public MemoManager MemoManager
        {
            get { return this._memoManager; }
        }
        public StatsManager StatsManager
        {
            get { return this._statsManager; }
        }
        public AchievementContainer AchievementContainer
        {
            get { return this._achievementContainer; }
        }
        public uint PlayerId
        {
            get { return this._playerId; }
        }
        public uint GetUserId
        {
            get { return this._userId; }
        }
        public string Name
        {
            get { return this._name; }
        }
        public uint Rank
        {
            get { return this._rank; }
        }
        public uint Experience
        {
            get { return this._experience; }
        }
        public uint Level
        {
            get { return this._level; }
            set { this._level = value; }
        }
        public byte Prestige
        {
            get { return this._prestige; }
        }
        public ushort Character
        {
            get { return this._character; }
            set { this._character = value; }
        }
        public bool Online
        {
            get { return this._isOnline; }
            set { this._isOnline = value; }
        }
        public uint Don
        {
            get { return this._don; }
        }
        public uint Cash
        {
            get { return this._cash; }
        }
        public uint Coins // NOTE: golden coin = 1_00_00 'coins' ?
        {
            get { return this._coins; }
        }
        public DateTime LoginTime
        {
            get { return this._loginTime; }
        }
        public bool Exists
        {
            get { return this._exists; }
        }
        public SquarePlayer SquarePlayer
        {
            get { return this._squarePlayer; }
        }
        public RoomPlayer RoomPlayer
        {
            get { return this._roomPlayer; }
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

        public void Update()
        {
            // TODO: _character, _don, _cash, _coins, _level, _prestige, _experience, _playerId
        }
        public void RemoveDon(uint count)
        {
            if (this._don <= count)
                this._don = 0;
            else
                this._don -= count;

            Update();
        }
        public void AddDon(uint count)
        {
            this._don += count;

            Update();
        }
        public void RemoveCash(uint count)
        {
            if (this._cash <= count)
                this._cash = 0;
            else
                this._cash -= count;

            Update();
        }
        public void AddCash(uint count)
        {
            this._cash += count;

            Update();
        }
        public void RemoveCoins(uint count)
        {
            if (this._coins <= count)
                this._coins = 0;
            else
                this._coins -= count;

            Update();
        }
        public void AddCoins(uint count)
        {
            this._coins += count;

            Update();
        }
    }
}
