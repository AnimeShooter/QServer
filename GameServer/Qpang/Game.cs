using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Qserver.GameServer.Database;
using Qserver.GameServer.Database.Repositories;
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
        private BanManager _banManager;
        private ChannelManager _channelManager;
        private ShopManager _shopManager;
        private SquareManager _squareManager;
        private CacheManager _cacheManager;
        private ChatManager _chatManager;
        private RoomManager _roomManager;
        private WeaponManager _weaponManager;
        private SpawnManager _spawnManager;
        private SkillManager _skillManager;
        private AchievementManager _achievementManager;
        private LevelManager _levelManger;
        private CraneManager _craneManager;

        private Leaderboard _leaderboard;

        // Database managers
        private ChannelsRepository _channelsRepository;
        private CraneRepository _craneRepository;
        private LevelRepository _levelRepository;
        private MapsRepository _mapsRepository;
        private PlayersRepository _playersRepository;
        private ItemsRepository _itemsRepository;
        private UsersRepository _usersRepository;

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
        public LevelRepository LevelRepository
        {
            get { return this._levelRepository; }
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
            this._levelRepository = new LevelRepository(DatabaseManager.MySqlFactory);
            this._mapsRepository = new MapsRepository(DatabaseManager.MySqlFactory);
            this._playersRepository = new PlayersRepository(DatabaseManager.MySqlFactory);
            this._itemsRepository = new ItemsRepository(DatabaseManager.MySqlFactory);
            this._usersRepository = new UsersRepository(DatabaseManager.MySqlFactory);

            // share
            Instance = this;

            // init TNL events
            GameConnection.RegisterNetClassReps();
            ///EventRegister.RegisterTNLEvents();

            // init managers
            this._channelManager = new ChannelManager();
            this._shopManager = new ShopManager();
            this._squareManager = new SquareManager();
            this._chatManager = new ChatManager(); // TODO: commands
            this._roomManager = new RoomManager();
            this._roomManager.Create("Vet, Cool en Fun!", 4, GameMode.Mode.TDM, 0x0100007F);
            this._roomManager.Create("Kim kAm qPong?", 8, GameMode.Mode.PTE, 0x0100007F);

            this._weaponManager = new WeaponManager();
            this._spawnManager = new SpawnManager(); // TODO, DB
            this._skillManager = new SkillManager(); // TODO
            this._levelManger = new LevelManager(); // TODO-ish
            this._craneManager = new CraneManager();
            this._leaderboard = new Leaderboard(); // TODO
            this._roomServer = new RoomServer(); // TODO
        }

        public void Tick()
        {
            while(true)
            {
                try
                {

                }catch(Exception e)
                {
                    Log.Message(LogType.ERROR, "Fatal error: " + e.ToString());
                }
                Thread.Sleep(1);
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
                    this._players[player.PlayerId].Close();
                }
                
                // cache manger
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
            lock(this._lock)
            {
                if(this._players.ContainsKey(playerId))
                    return this._players[playerId];
                return null;
            }
        }
        public Player GetPlayer(string name)
        {
            lock (this._lock)
            {
                if (this._playersByName.ContainsKey(name))
                    return this._playersByName[name];
                return null;
            }
        }

        // TODO: event handles to kick player on connection close

    }
}
