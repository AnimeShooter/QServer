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
            // Master always starts as public enemy NOTE: this does not work?
            var players = roomSession.GetPlayers();
            foreach (var p in players)
                if (p.Player.PlayerId == roomSession.Room.MasterId)
                    roomSession.NextPublicEnemy = p;

            roomSession.PublicEnemeyReset = Util.Util.Timestamp() + 10; // now + wait/respawn time

            base.OnStart(roomSession);
        }

        public override void Tick(RoomSession roomSession)
        {
            uint currTime = Util.Util.Timestamp();
            if (roomSession.PublicEnemeyReset == 0)
            {
                roomSession.PublicEnemy = null;
                roomSession.PublicEnemeyReset = currTime;
            }
            else if(roomSession.PublicEnemeyReset + 5 <= currTime && roomSession.PublicEnemy == null)
            {
                RoomSessionPlayer newEnemy = null;
                if (roomSession.NextPublicEnemy != null)
                    newEnemy = roomSession.NextPublicEnemy;
                else
                    newEnemy = roomSession.FindNextPublicEnemy();

                newEnemy.WeaponManager.Reset(); // fix for respawn
                roomSession.PublicEnemy = newEnemy;
            }

            base.Tick(roomSession);
        }

        public override void OnPlayerSync(RoomSessionPlayer session)
        {
            var roomSession = session.RoomSession;
            if (roomSession != null && roomSession.PublicEnemy != null)
            {
                //if (roomSession.PublicEnemy.Player.PlayerId != session.Player.PlayerId)
                //    session.Post(new GCGameState(session.Player.PlayerId, (uint)CGGameState.State.PREY_COUNT_START, 0));

                session.Post(new GCGameState(roomSession.PublicEnemy.Player.PlayerId, (uint)CGGameState.State.PREY_TRANFORM, roomSession.PublicEnemy.Health));
                //session.Post(new GCGameState(roomSession.PublicEnemy.Player.PlayerId, (uint)CGGameState.State.PREY_TRANFORM_READY, roomSession.PublicEnemy.Health));
                session.Post(new GCGameState(roomSession.PublicEnemy.Player.PlayerId, (uint)CGGameState.State.PREY_SELECT, roomSession.PublicEnemy.Health));
            }
        }

        public override void OnPlayerKill(RoomSessionPlayer killer, RoomSessionPlayer target, Weapon weapon, byte hitLocation)
        {
            // TODO: check if target was Prey and start new counter
            var roomSession = target.RoomSession;
            if (roomSession == null)
                return;

            if (roomSession.PublicEnemy != target)
                killer.Score++;
            else if (roomSession.PublicEnemy == target)
            {
                killer.Kills++;
                //roomSession.NextPublicEnemy = killer; // TODO: randomize?
                roomSession.NextPublicEnemy = roomSession.FindNextPublicEnemy();
            }

            killer.Kills--; // pre-undo base kill gain

            // TODO2: keep track of kills
            base.OnPlayerKill(killer, target, weapon, hitLocation);
        }
    }
}
