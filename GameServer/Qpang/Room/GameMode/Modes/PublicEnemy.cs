using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class PublicEnemy : GameMode
    {
        public override bool IsMissionMode()
        {
            return true;
        }

        public override bool IsTeamMode()
        {
            return false;
        }

        public override void OnStart(RoomSession roomSession)
        {
            //var players = roomSession.GetPlayers();
            //if(players.Count > 0)
            //    players.

            base.OnStart(roomSession);
        }

        public override void Tick(RoomSession roomSession)
        {
            if(roomSession.PublicEnemy == null)
            {
                var players = roomSession.GetPlayers();
                if (players.Count > 0)
                    roomSession.PublicEnemy = players[0];
            }
                

            base.Tick(roomSession);
        }

        public override void OnPlayerKill(RoomSessionPlayer killer, RoomSessionPlayer target, Weapon weapon, byte hitLocation)
        {
            // TODO: check if target was Prey and start new counter
            var roomSession = target.RoomSession;
            if (roomSession == null)
                return;

            if (roomSession.PublicEnemy != target)
                killer.Kills++;
            else if (roomSession.PublicEnemy == target)
            {
                killer.Score++;
                roomSession.PublicEnemy = killer;
            }

            killer.Kills--; // rebase?

            // TODO2: keep track of kills
            base.OnPlayerKill(killer, target, weapon, hitLocation);
        }
    }
}
