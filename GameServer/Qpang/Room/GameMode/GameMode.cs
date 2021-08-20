using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class GameMode
    {
        public enum Mode
        {
            DM = 1,
            TDM = 2,
            PTE = 3,
            VIP = 4,
            PRACTICE = 5,
            PREY = 8,
            PVE = 9,
        };

        public virtual bool IsTeamMode()
        {
            return false;
        }

        public virtual bool IsMissionMode()
        {
            return false;
        }

        public virtual bool IsPvE()
        {
            return false;
        }

        public virtual void Tick(RoomSession roomSession) { }

        public virtual void OnApply(Room room)
        {
            room.BalancePlayers();
        }

        public virtual void OnStart(RoomSession roomSession) { }

        public virtual void OnPlayerSync(RoomSessionPlayer session) { }

        public virtual void OnPlayerKill(RoomSessionPlayer killer, RoomSessionPlayer target, Weapon weapon, byte hitLocation)
        {
            if (killer == null || target == null)
                return;

            var roomSession = killer.RoomSession;
            bool TeamMode = roomSession.GameMode.IsTeamMode();
            bool Suicided = killer == target;
            bool SameTeam = killer.Team == target.Team;

            if(Suicided)
                killer.AddDeath();
            else
            {
                //var cashEarned = 
                // NOTE: may add cash earning?
                killer.AddKill();
                target.AddDeath();

                if(!IsMissionMode())
                {
                    if (!SameTeam && TeamMode)
                        killer.AddScore();
                    else if (!TeamMode)
                        killer.AddScore();
                }
                killer.AddStreak();
            }

            target.ResetStreak();
            target.EffectManager.Clear(); // TODO
            target.StartPrespawn();

            // NOTE: balance?
            killer.SkillManager.AddSkillPoints(40);

            var killerStats = killer.Player.StatsManager;
            var targetStats = target.Player.StatsManager;

            if(hitLocation == 0)
            {
                killerStats.AddHeadshotKills();
                targetStats.AddHeadshotDeath();
            }

            if(killer.Team == target.Team && TeamMode)
            {
                killerStats.AddTeamKills();
                killerStats.AddTeamDeaths();
            }

            switch(weapon.WeaponType)
            {
                case WeaponType.MELEE:
                    killerStats.AddMeleeKills();
                    break;
                case WeaponType.RIFLE:
                    killerStats.AddGunKills();
                    break;
                case WeaponType.LAUNCHER:
                    killerStats.AddLauncherKills();
                    break;
                case WeaponType.BOMB:
                    killerStats.AddBombKills();
                    break;
            }

            if (roomSession.CanFinish())
                roomSession.Finish();

        }

    }
}
