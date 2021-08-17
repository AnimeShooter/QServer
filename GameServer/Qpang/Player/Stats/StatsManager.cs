using System;

namespace Qserver.GameServer.Qpang
{
    public class StatsManager
    {
        private Player _player;

        private uint _kills;
        private uint _deaths;
        private uint _normalWon;
        private uint _normalLost;
        private uint _normalDrew;
        private uint _missionWon;
        private uint _missionLost;
        private uint _missionDrew;
        private uint _slackerPoints;
        private uint _playTime;
        private uint _meleeKills;
        private uint _gunKills;
        private uint _launcherKills;
        private uint _bombKills;
        private uint _headshotKills;
        private uint _headshotDeaths;
        private uint _teamKills;
        private uint _teamDeaths;
        private uint _eventItemPickUps;

        // TODO: add PvE stats for achievement manager!

        // BossClear, BronzeCoin, cameraKiller, chessoflege, chessofmaster, defnderofdiorama/2, essemceDefender/2, goldCoin

        public uint Kills
        {
            get { return this._kills; }
        }
        public uint Deaths
        {
            get { return this._deaths; }
        }
        public uint NormalWins
        {
            get { return this._normalWon; }
        }
        public uint NormalLosses
        {
            get { return this._normalLost; }
        }
        public uint NormalDrews
        {
            get { return this._normalDrew; }
        }
        public uint MissionWins
        {
            get { return this._missionWon; }
        }
        public uint MissionLosses
        {
            get { return this._missionLost; }
        }
        public uint MissionDrews
        {
            get { return this._normalDrew; }
        }
        public uint PlayTime
        {
            get { return this._playTime; }
        }
        public uint PlayTimeInMinutes
        {
            get { return this._playTime/60; }
        }
        public uint SlackerPoints
        {
            get { return this._slackerPoints; }
        }
        public uint MeleeKills
        {
            get { return this._meleeKills; }
        }
        public uint GunKills
        {
            get { return this._gunKills; }
        }
        public uint LauncherKills
        {
            get { return this._launcherKills; }
        }
        public uint BombKills
        {
            get { return this._bombKills; }
        }
        public uint HeadshotKills
        {
            get { return this._headshotKills; }
        }
        public uint HeadshotDeaths
        {
            get { return this._headshotDeaths; }
        }
        public uint TeamKills
        {
            get { return this._teamKills; }
        }
        public uint TeamDeaths
        {
            get { return this._teamDeaths; }
        }
        public StatsManager(Player player)
        {
            this._player = player;
            var stats = Game.Instance.PlayersRepository.GetPlayerStats(this._player.PlayerId).Result;

            this._kills = stats.kills;
            this._deaths = stats.deaths;
            this._normalWon = stats.n_won;
            this._normalDrew = stats.n_drew;
            this._normalLost = stats.n_lost;
            this._missionWon = stats.m_won;
            this._missionDrew = stats.m_drew;
            this._missionLost = stats.m_lost;
            this._playTime = stats.playtime;
            this._slackerPoints = stats.slacker_points;
            this._meleeKills = stats.melee_kills;
            this._gunKills = stats.gun_kills;
            this._launcherKills = stats.launcher_kills;
            this._bombKills = stats.bomb_kills;
            this._headshotKills = stats.headshot_kills;
            this._headshotDeaths = stats.headshot_deaths;
            this._teamKills = stats.team_kills;
            this._teamDeaths = stats.team_death;
            this._eventItemPickUps = stats.event_item_pickups;
        }

        public void Save()
        {
            if (this._player.TestRealm)
                return;

            Game.Instance.PlayersRepository.UpdatePlayerStats(this._player).GetAwaiter().GetResult();
        }

        public void Apply(RoomSessionPlayer player)
        {
            AddKills(player.Kills);
            AddDeaths(player.Deaths);
            AddPlaytime(player.PlayTime);
            this._eventItemPickUps += player.EventItemPickUps;

            Save();
        }
        public void AddKills(uint count)
        {
            this._kills += count;
        }

        public void AddDeaths(uint count)
        {
            this._deaths += count;
        }

        public void ClearKD()
        {
            this._kills = 0;
            this._deaths = 0;
            this._teamKills = 0;
            this._teamDeaths = 0;
            this._meleeKills = 0;
            this._gunKills = 0;
            this._launcherKills = 0;
            this._bombKills = 0;
            this._headshotKills = 0;
            this._headshotDeaths = 0;

            Save();
        }

        public void AddPlaytime(uint time)
        {
            this._playTime += time;
        }

        public void AddNormalWin()
        {
            this._normalWon++;
        }
        public void AddNormalLoss()
        {
            this._normalLost++;
        }
        public void AddNormalDraw()
        {
            this._normalDrew++;
        }
        public void AddMissionWin()
        {
            this._missionWon++;
        }
        public void AddMissionLoss()
        {
            this._missionLost++;
        }
        public void AddMissionDraw()
        {
            this._missionDrew++;
        }
        public void ClearWL()
        {
            this._normalWon = 0;
            this._normalLost = 0;
            this._normalDrew = 0;
            this._missionWon = 0;
            this._missionLost = 0;
            this._missionDrew = 0;

            Save();
        }
        public void AddSlackerPoint()
        {
            this._slackerPoints++;
        }
        public void AddMeleeKills(uint count = 1)
        {
            this._meleeKills += count;
        }
        public void AddGunKills(uint count = 1)
        {
            this._gunKills += count;
        }
        public void AddLauncherKills(uint count = 1)
        {
            this._launcherKills += count;
        }
        public void AddBombKills(uint count = 1)
        {
            this._bombKills += count;
        }
        public void AddHeadshotKills(uint count = 1)
        {
            this._headshotKills += count;
        }
        public void AddHeadshotDeath(uint count = 1)
        {
            this._headshotDeaths += count;
        }
        public void AddTeamKills(uint count = 1)
        {
            this._teamKills += count;
        }
        public void AddTeamDeaths(uint count = 1)
        {
            this._teamDeaths += count;
        }
    }
}
