using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Qserver.Database;
using Qserver.Database.Repositories;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Managers;
using Qserver.Util;

namespace Qserver.GameServer.Qpang
{
    public enum GameState
    {
        LOGIN = 1,
        WAITROOM,
        GAME,
        LOADING,
        TUTORIAL,
        UI_SQUARE_STATE
    }

    public enum GameUIState
    {
        NONE_STATE = 0,
        LOGO_STATE,
        LOGIN_STATE,
        WAITROOM_STATE,
        LOAD_STATE,
        GAME_STATE,
        MATCH_STATE,
        STRUGGLE_STATE = 7,
        TUTORIAL_STATE = 9,
        PRACTICE_STATE = 8,
        SQUARE_STATE = 11,
        PVE_STATE = 10
    }

    public class Game
    {
        public static Game Instance;

        private RoomServer _roomServer;
        private BanManager _banManager; // TODO
        private ChannelManager _channelManager;
        private ShopManager _shopManager;
        private SquareManager _squareManager;
        private CacheManager _cacheManager; 
        private ChatManager _chatManager; // TODO: commands
        private RoomManager _roomManager;
        private WeaponManager _weaponManager;
        private SpawnManager _spawnManager; // TODO
        private SkillManager _skillManager;
        private AchievementManager _achievementManager; // TODO
        private LevelManager _levelManger;
        private CraneManager _craneManager;
        private CouponManager _couponManager;

        private Leaderboard _leaderboard;

        // Database managers
        private ChannelsRepository _channelsRepository;
        private CraneRepository _craneRepository;
        private LevelsRepository _levelsRepository;
        private MapsRepository _mapsRepository;
        private PlayersRepository _playersRepository;
        private ItemsRepository _itemsRepository;
        private UsersRepository _usersRepository;
        private MemosRepository _memosRepository;
        private FriendsRepository _friendsRepository;
        private CouponsRepository _couponsRepository;
        private SpawnsRepository _spawnsRepository;

        private QpangServer _lobbyServer; // lobby?
        private QpangServer _squareServer;

        private object _lock;

        private Dictionary<uint, Player> _players;
        private Dictionary<string, Player> _playersByName;

        public BanManager BanManager
        {
            get { return this._banManager; }
        }
        public ChannelManager ChannelManager
        {
            get { return this._channelManager; }
        }
        public ShopManager ShopManager
        {
            get { return this._shopManager; }
        }
        public SquareManager SquareManager
        {
            get { return this._squareManager; }
        }
        public CacheManager CacheManager
        {
            get { return this._cacheManager; }
        }
        public ChatManager ChatManager
        {
            get { return this._chatManager; }
        }
        public RoomManager RoomManager
        {
            get { return this._roomManager; }
        }
        public WeaponManager WeaponManager
        {
            get { return this._weaponManager; }
        }
        public SpawnManager SpawnManager
        {
            get { return this._spawnManager; }
        }
        public SkillManager SkillManager
        {
            get { return this._skillManager; }
        }
        public AchievementManager AchievementManager
        {
            get { return this._achievementManager; }
        }
        public LevelManager LevelManager
        {
            get { return this._levelManger; }
        }
        public CraneManager CraneManger
        {
            get { return this._craneManager; }
        }
        public CouponManager CouponManager
        {
            get { return this._couponManager; }
        }
        public Leaderboard Leaderboard
        {
            get { return this._leaderboard; }
        }
        public RoomServer RoomServer
        {
            get { return this._roomServer; }
        }

        // db
        public ChannelsRepository ChannelsRepository
        {
            get { return this._channelsRepository; }
        }
        public CraneRepository CraneRepository
        {
            get { return this._craneRepository; }
        }
        public LevelsRepository LevelRepository
        {
            get { return this._levelsRepository; }
        }
        public MapsRepository MapsRepository
        {
            get { return this._mapsRepository; }
        }
        public PlayersRepository PlayersRepository
        {
            get { return this._playersRepository; }
        }
        public ItemsRepository ItemsRepository
        {
            get { return this._itemsRepository; }
        }
        public UsersRepository UsersRepository
        {
            get { return this._usersRepository; }
        }
        public MemosRepository MemoRepository
        {
            get { return this._memosRepository; }
        }
        public FriendsRepository FriendsRepository
        {
            get { return this._friendsRepository; }
        }
        public CouponsRepository CouponsRepository
        {
            get { return this._couponsRepository; }
        }
         public SpawnsRepository SpawnsRepository
        {
            get { return this._spawnsRepository; }
        }

        public Game(bool lobby)
        {
            this._lock = new object();

            this._players = new Dictionary<uint, Player>();
            this._playersByName = new Dictionary<string, Player>();

            // init servers
            if (lobby)
            {
                this._lobbyServer = new QpangServer(Settings.SERVER_PORT_LOBBY);
                this._lobbyServer.Start();
            }

            this._squareServer = new QpangServer(Settings.SERVER_PORT_SQUARE);
            this._squareServer.Start();

            // init databases
            this._channelsRepository = new ChannelsRepository(DatabaseManager.MySqlFactory);
            this._craneRepository = new CraneRepository(DatabaseManager.MySqlFactory);
            this._levelsRepository = new LevelsRepository(DatabaseManager.MySqlFactory);
            this._mapsRepository = new MapsRepository(DatabaseManager.MySqlFactory);
            this._playersRepository = new PlayersRepository(DatabaseManager.MySqlFactory);
            this._itemsRepository = new ItemsRepository(DatabaseManager.MySqlFactory);
            this._usersRepository = new UsersRepository(DatabaseManager.MySqlFactory);
            this._memosRepository = new MemosRepository(DatabaseManager.MySqlFactory);
            this._friendsRepository = new FriendsRepository(DatabaseManager.MySqlFactory);
            this._couponsRepository = new CouponsRepository(DatabaseManager.MySqlFactory);
            this._spawnsRepository = new SpawnsRepository(DatabaseManager.MySqlFactory);

            // share
            Instance = this;

            // init TNL events
            GameConnection.RegisterNetClassReps();

            // init managers
            this._channelManager = new ChannelManager();
            this._shopManager = new ShopManager();
            this._squareManager = new SquareManager();
            this._chatManager = new ChatManager(); // TODO: commands
            this._roomManager = new RoomManager();
            
            this._weaponManager = new WeaponManager();
            this._spawnManager = new SpawnManager();
            this._skillManager = new SkillManager(); // TODO
            this._levelManger = new LevelManager(); // TODO-ish
            this._craneManager = new CraneManager();
            this._couponManager = new CouponManager();
            this._cacheManager = new CacheManager();
            this._leaderboard = new Leaderboard();
            this._leaderboard.Refresh(); // initial update (TODO: refresh  every 12/24h?)
            this._roomServer = new RoomServer(); 
        }

        public void Tick()
        {
            uint lastLeaderRefresh = Util.Util.Timestamp();
            while(true)
            {
                try
                {
                    uint currTime = Util.Util.Timestamp();

                    // Update leaderboard
                    if(lastLeaderRefresh + (60 * 60 * 24 * 1) < currTime) // 24h
                    {
                        this._leaderboard.Refresh();
                        lastLeaderRefresh = currTime;
                    }

                    // NOTE: Enabled/Disable Exp weekend, broadcast stuff, reset weekly leaderboard?


                    // take some rest? 
                    Thread.Sleep(250); 
                }
                catch(Exception e)
                {
                    Log.Message(LogType.ERROR, "Fatal error: " + e.ToString());
                }
            }
        }

        public List<Player> PlayersList()
        {
            lock(this._lock)
            {
                List<Player> players = new List<Player>();
                foreach (var player in this._players)
                    players.Add(player.Value);
                return players;
            }
        }

        public void RemoveClient(Player player)
        {
            if (player == null)
                return;

            lock (this._lock)
            {
                if(this._players.ContainsKey(player.PlayerId))
                    _players.Remove(player.PlayerId);
                if (this._playersByName.ContainsKey(player.Name))
                    _playersByName.Remove(player.Name);

                this._cacheManager.PlayerCacheManager.Cache(player);
            }
        }

        public Player CreatePlayer(ConnServer conn, uint playerId)
        {
            Player player = new Player(playerId);

            player.LobbyConnection = conn;
            conn.Player = player;

            lock(this._lock)
            {
                if(this._players.ContainsKey(player.PlayerId))
                {
                    this._players[player.PlayerId].SendLobby(LobbyManager.Instance.DuplicateLogin());
                    if(this._players.ContainsKey(player.PlayerId)) 
                        this._players[player.PlayerId].Close(); // only close if its still there
                }

                this._cacheManager.PlayerCacheManager.Invalidate(playerId);
                
                this._players[player.PlayerId] = player;
                this._playersByName[player.Name] = player;
            }

            return player;
        }

        public Player GetOnlinePlayer(string name)
        {
            lock(this._lock)
            {
                if (this._playersByName.ContainsKey(name))
                    return this._playersByName[name];
                return null;
            }
        }

        public Player GetOnlinePlayer(uint playerId)
        {
            lock (this._lock)
            {
                if (this._players.ContainsKey(playerId))
                    return this._players[playerId];
                return null;
            }
        }

        public void OnSquareConnClose(SquareManager conn)
        {
            // TODO: handle closing
        }

        public Player GetPlayer(uint playerId)
        {
            var player = GetOnlinePlayer(playerId);
            if (player != null)
                return player;

            var cachedPlayer = this._cacheManager.PlayerCacheManager.Get(playerId);
            if (cachedPlayer != null)
                return cachedPlayer;

            return this._cacheManager.PlayerCacheManager.Cache(playerId);
        }
        public Player GetPlayer(string name)
        {
            var player = GetOnlinePlayer(name);
            if (player != null)
                return player;

            var cachedPlayer = this._cacheManager.PlayerCacheManager.Get(name);
            if (cachedPlayer != null)
                return cachedPlayer;

            return this._cacheManager.PlayerCacheManager.Cache(name); // TODO: add DB query
        }
    }
}
