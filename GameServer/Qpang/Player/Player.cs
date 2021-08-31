using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Packets;
using Qserver.GameServer.Network.Managers;
using Qserver.Database.Repositories;
using Qserver.Database;

namespace Qserver.GameServer.Qpang
{
    public struct PlayerAPI
    {
        public string Name;
        public uint Experience;
        public uint Level;
        public uint Kills;
        public uint Deaths;
        public uint MeleeKills;
        public uint GunKills;
        public uint LauncherKills;
        public uint BombKills;
        public bool Gm;
        public bool TestRealm;
    }

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
        private byte _level;
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
        private bool _testRealm;
        private uint _lastAntiCheatBeat;
        private bool _isBot;

        private uint _currentSquareId;

        private SquarePlayer _squarePlayer;
        private RoomPlayer _roomPlayer;

        private ConnServer _squareConnection;
        private ConnServer _lobbyConnection;

        private PlayersRepository _playerRepository;

        private object _lock;
        private object _lobbyLock;
        private object _squareLock;

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
        public object Lock
        {
            get { return this._lock; }
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
            set { this._name = value; }
        }
        public bool IsBot
        {
            get { return this._isBot; }
        }
        public byte Rank
        {
            get { return this._rank; }
            set { this._rank = value; }
        }
        public uint Experience
        {
            get { return this._experience; }
        }
        public byte Level
        {
            get { return this._level; }
            set
            {
                this._level = value;
                if (this._squarePlayer != null)
                    lock (this._squarePlayer.Lock)
                        this._squarePlayer.Square.SendPacket(Network.SquareManager.Instance.UpdatePlayerLevel(this._playerId, this._level));
            }
        }
        public byte Prestige
        {
            get { return this._prestige; }
        }
        public ushort Character
        {
            get { return this._character; }
            set
            {
                this._character = value;
                SendLobby(LobbyManager.Instance.UpdateCharacter(value));
            }
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
        public uint Coins // NOTE: golden coin = 1_0_0 'coins' ?
        {
            get { return this._coins; }
        }
        public bool TestRealm
        {
            get { return this._testRealm; }
            set { this._testRealm = value; }
        } 
        public uint AntiCheat
        {
            get { return this._lastAntiCheatBeat; }
            set { this._lastAntiCheatBeat = value; }
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
            set { this._squarePlayer = value; }
        }
        public RoomPlayer RoomPlayer
        {
            get { return this._roomPlayer; }
            set { this._roomPlayer = value; }
        }
        public ConnServer SquareConnection
        {
            get { return this._squareConnection; }
            set { this._squareConnection = value; }
        } 
        public ConnServer LobbyConnection
        {
            get { return this._lobbyConnection; }
            set { this._lobbyConnection = value; }
        }
        public Player(uint playerId)
        {
            this._lock = new object();
            this._lobbyLock = new object();
            this._squareLock = new object();
            this._playerId = playerId;
            this._loginTime = DateTime.UtcNow;

            var playerData = Game.Instance.PlayersRepository.GetPlayer(playerId).Result;

            this._name = playerData.name;
            this._level = playerData.level;
            this._rank = playerData.rank;
            this._prestige = playerData.prestige;
            this._character = playerData.default_character;
            this._userId = playerData.user_id;
            this._coins = playerData.coins;
            this._don = playerData.don;
            this._cash = playerData.cash;
            this._experience = playerData.experience;
            this._isMuted = playerData.is_muted == 1;

            this._equipmentManager = new EquipmentManager(this); // tok
            this._inventoryManager = new InventoryManager(this);
            this._friendManager = new FriendManager(this);
            this._memoManager = new MemoManager(this);
            this._statsManager = new StatsManager(this); // ok
            this._achievementContainer = new AchievementContainer(playerId);  // TODO DB!
            this._isOnline = true;        
        }

        public Player(string botName)
        {
            this._isBot = true;
            this._lock = new object();
            this._lobbyLock = new object();
            this._squareLock = new object();

            Random rnd = new Random();
            uint id = (uint)rnd.Next(0x1FFFFF, 0xFFFFFF);
            this._playerId = id;
            this._level = 1;
            this._rank = 5;
            this._prestige = 1;

            if (botName == "")
                this._name = $"[{id.ToString("X6")}]Bot";
            else
            {
                this._name = botName;
                this._level = (byte)rnd.Next(2, 23);
            }

            ushort[] characters = new ushort[]
            {
                //1, 329, 836, 602, 328, 851, 850, 579, 578, 343, 333
                //1, 329, 836, 602, 328, 
                851, 850, 579, 578, 343, 333
            };

            this._character = characters[rnd.Next(0, characters.Length)];
            this._userId = id;
            this._experience = 0;
            this._isMuted = false;

            // TODO: randomize all?
            this._equipmentManager = new EquipmentManager(this, true); // tok
            this._inventoryManager = new InventoryManager(this, true);
            //this._friendManager = new FriendManager(this);
            //this._memoManager = new MemoManager(this);
            this._statsManager = new StatsManager(this, true); // ok
            this._achievementContainer = new AchievementContainer(id);  // TODO DB!
            //this._isOnline = true;
        }

        public PlayerAPI ToAPI()
        {
            return new PlayerAPI()
            {
                Name = this.Name,
                Deaths = this._statsManager.Deaths,
                Kills = this._statsManager.Kills,
                Experience = this._experience,
                Level = this._level,
                MeleeKills = this._statsManager.MeleeKills,
                GunKills = this._statsManager.GunKills,
                LauncherKills = this._statsManager.LauncherKills,
                BombKills = this._statsManager.BombKills,
                Gm = this.Rank == 3,
                TestRealm = this._testRealm
            };
        }

        public void Broadcast(string message)
        {
            if (this._lobbyConnection == null)
                return;

            lock(this._lobbyConnection)
                this._lobbyConnection.Send(LobbyManager.Instance.Broadcast(message));
        }

        public void EnterSquare(SquarePlayer squarePlayer)
        {
            if (squarePlayer == null)
                return;

            this._squarePlayer = squarePlayer;
            SetOnlineStatus(true);
            SendSquare(Network.SquareManager.Instance.JoinSquareSuccess(squarePlayer));
        }

        public void SendSquare(PacketWriter packet)
        {
            lock(this._squareLock)
                if (this._squareConnection != null)
                    this._squareConnection.Send(packet);
        }

        public void SendLobby(PacketWriter packet)
        {
            lock (this._lobbyLock)
                if(this._lobbyConnection != null)
                    this._lobbyConnection.Send(packet);
        }

        public void LeaveSquare()
        {
            this._squarePlayer = null;
        }

        public void Close()
        {
            lock(this._lock)
            {
                if (this._isClosed)
                    return;

                this._isOnline = false;
                this._isClosed = true;

                if (this._lobbyConnection != null)
                    lock(this._lobbyLock)
                        this._lobbyConnection.CloseSocket();

                if (this._squareConnection != null)
                    lock(this._squareLock)
                        this._squareConnection.CloseSocket();

                // cancle if any
                Game.Instance.TradeManager.OnCancel(this);
            }

            if (this._roomPlayer != null)
                this._roomPlayer.Room.RemovePlayer(this._playerId); // remove from room

            SetOnlineStatus(false);

            this._inventoryManager.Close();
            this._equipmentManager.Close();
            this._friendManager.Close();

            if (this._squarePlayer != null)
                lock (this._squareLock)
                    this._squarePlayer.Square.Remove(this._playerId);

            Game.Instance.RemoveClient(this);

            Update();
        }

        public void Whisper(string name, string message)
        {
            SendLobby(LobbyManager.Instance.ReceiveWhisper(name, message));
        }
        public void SetOnlineStatus(bool status)
        {
            this._isOnline = status;
            if (status)
                this._friendManager.AppearOnline();
            else
                this._friendManager.AppearOffline();
        }
        public void Update()
        {
            if (this._testRealm)
                return;

            Game.Instance.PlayersRepository.UpdatePlayer(this).GetAwaiter().GetResult();
        }
        public void RemoveDon(uint count)
        {
            if (this._testRealm)
                return;

            if (this._don <= count)
                this._don = 0;
            else
                this._don -= count;

            Update();
        }
        public void AddDon(uint count)
        {
            if (this._testRealm)
                return;

            this._don += count;

            Update();
        }
        public void RemoveCash(uint count)
        {
            if (this._testRealm)
                return;

            if (this._cash <= count)
                this._cash = 0;
            else
                this._cash -= count;

            Update();
        }
        public void AddCash(uint count)
        {
            if (this._testRealm)
                return;

            this._cash += count;
            Update();
        }
        public void RemoveCoins(uint count)
        {
            if (this._testRealm)
                return;

            if (this._coins <= count)
                this._coins = 0;
            else
                this._coins -= count;

            Update();
        }
        public void AddCoins(uint count)
        {
            if (this._testRealm)
                return;

            this._coins += count;
            Update();
        }
        public void AddExp(uint count)
        {
            if (this._testRealm)
                return;

            this._experience += count;
            Update();
        }
    }
}
