using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    #region AchievementID
    public enum AchievementID : uint
    {
        WARRIOR = 11,
        FIGHTER = 12,
        SLAYER = 13,
        CHAMPION = 14,
        DESTROY_KING = 15,

        FRIEND = 21,
        LOVER = 22,
        ADDICT = 23,
        STOCKER = 24,
        MADNESS = 25,

        CHIEF = 51,
        HEAD = 52,
        LEADER = 53,
        BOSS = 54,
        HERO = 55,

        TUMBLER = 61,
        FAITHFUL = 62,
        SINGLE_MINDED = 63,
        IRON_WILL = 64,
        SUPERMAN = 65,

        TRICKY = 71,
        BRAIN = 72,
        ANALYST = 73,
        STAFF = 74,
        STRATEGIST = 75,

        NOVICE = 81,
        HARD_WORKING = 82,
        CHALLENGER = 83,
        TRANSCENDENT = 84,
        ART_SEEKER = 85,

        SHOOTING_STAR = 91,
        DEMOLITIONER = 101,
        PITCHER = 111,
        SWORD_MASTER = 121,

        HEAD_COLLECTOR = 131,
        BIG_HEAD = 141,


        MANAGER = 161,
        HEAD_CHIEF = 162,
        PRESIDENT = 163,
        CHAIRMAN = 164,
        GODS_SON = 165,

        LUCKY_HAND = 171,
        BRASS_HAND = 172,
        SILVER_HAND = 173,
        GOLDEN_HAND = 174,
        GODS_HAND = 175,

        SPY = 181,
        SUSPECT = 191,
    };
    #endregion

    public class AchievementManager
    {
        private Dictionary<AchievementID, AchievementCondition> _achievements;

        public AchievementManager()
        {
            this._achievements = new Dictionary<AchievementID, AchievementCondition>()
            {
                { AchievementID.WARRIOR, new KillCondition(1000) },
                { AchievementID.FIGHTER, new KillCondition(5000) },
                { AchievementID.SLAYER, new KillCondition(10000) },
                { AchievementID.CHAMPION, new KillCondition(20000) },
                { AchievementID.DESTROY_KING, new KillCondition(50000) },

                { AchievementID.FRIEND, new DeathCondition(1000) },
                { AchievementID.LOVER, new DeathCondition(5000) },
                { AchievementID.ADDICT, new DeathCondition(10000) },
                { AchievementID.STOCKER, new DeathCondition(20000) },
                { AchievementID.MADNESS, new DeathCondition(50000) },

                { AchievementID.CHIEF, new TeamWinCondition(100) },
                { AchievementID.HEAD, new TeamWinCondition(200) },
                { AchievementID.LEADER, new TeamWinCondition(500) },
                { AchievementID.BOSS, new TeamWinCondition(1000) },
                { AchievementID.HERO, new TeamWinCondition(10000) },

                { AchievementID.TUMBLER, new TeamLoseCondition(100) },
                { AchievementID.FAITHFUL, new TeamLoseCondition(200) },
                { AchievementID.SINGLE_MINDED, new TeamLoseCondition(500) },
                { AchievementID.IRON_WILL, new TeamLoseCondition(1000) },
                { AchievementID.SUPERMAN, new TeamLoseCondition(10000) },

                { AchievementID.TRICKY, new MissionWinCondition(100) },
                { AchievementID.BRAIN, new MissionWinCondition(200) },
                { AchievementID.ANALYST, new MissionWinCondition(500) },
                { AchievementID.STAFF, new MissionWinCondition(1000) },
                { AchievementID.STRATEGIST, new MissionWinCondition(10000) },

                { AchievementID.NOVICE, new MissionLoseCondition(100) },
                { AchievementID.HARD_WORKING, new MissionLoseCondition(200) },
                { AchievementID.CHALLENGER, new MissionLoseCondition(500) },
                { AchievementID.TRANSCENDENT, new MissionLoseCondition(1000) },
                { AchievementID.ART_SEEKER, new MissionLoseCondition(10000) },

                { AchievementID.SHOOTING_STAR, new GunKillCondition(1000) },
                { AchievementID.DEMOLITIONER, new LauncherKillCondition(1000) },
                { AchievementID.PITCHER, new BombKillCondition(1000) },
                { AchievementID.SWORD_MASTER, new MeleeKillCondition(1000) },

                { AchievementID.HEAD_COLLECTOR, new HeadshotKillCondition(5000) },
                { AchievementID.BIG_HEAD, new HeadshotDeathCondition(1000) },

                { AchievementID.MANAGER, new StreakCondition(10) },
                { AchievementID.HEAD_CHIEF, new StreakCondition(20) },
                { AchievementID.PRESIDENT, new StreakCondition(30) },
                { AchievementID.CHAIRMAN, new StreakCondition(35) }, // 40
                { AchievementID.GODS_SON, new StreakCondition(40) }, // 50

                { AchievementID.LUCKY_HAND, new MultiKillCondition(2) },
                { AchievementID.BRASS_HAND, new MultiKillCondition(3) }, 
                { AchievementID.SILVER_HAND, new MultiKillCondition(5) }, // 5
                { AchievementID.GOLDEN_HAND, new MultiKillCondition(8) }, // 10
                { AchievementID.GODS_HAND, new MultiKillCondition(10) }, // 15

                { AchievementID.SPY, new TeamKillCondition(100) }, 
                { AchievementID.SUSPECT, new TeamKillCondition(100) }

            };
        }

        public void OnPlayerFinish(RoomSessionPlayer session)
        {
            var player = session.Player;
            var achievementContainer = player.AchievementContainer;

            foreach (var achievement in this._achievements)
                if (achievement.Value.IsMatch(session))
                    achievementContainer.Unlock((uint)achievement.Key);
        }
    }
}
