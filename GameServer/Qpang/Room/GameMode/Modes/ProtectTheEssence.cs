using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class ProtectTheEssence : GameMode
    {
        public override bool IsMissionMode()
        {
            return true;
        }

        public override bool IsTeamMode()
        {
            return true;
        }

        public override void OnApply(Room room)
        {
            room.PointsGame = false;
            room.ScoreTime = 10;

            base.OnApply(room);
        }

        public override void Tick(RoomSession roomSession)
        {
            var essHolder = roomSession.EssenceHolder;
            if (essHolder != null)
                essHolder.AddScore();

            var EligiblePoints = roomSession.GetTimeLeftInSeconds();
            var TeamBluePoints = roomSession.BluePoints;
            var TeamYellowPoints = roomSession.YellowPoints;

            // check early finish
            if ((TeamBluePoints + EligiblePoints < TeamYellowPoints) || (TeamYellowPoints + EligiblePoints < TeamBluePoints))
                roomSession.Finish();

            if (!roomSession.IsEssenceReset && roomSession.IsEssenceDropped)
                if (roomSession.GetElapsedEssenceDropTime() > 20) // Essence Reset Timer
                    roomSession.ResetEssence(); // reset position
        }

        public override void OnPlayerSync(RoomSessionPlayer sessionPlayer)
        {
            var roomSession = sessionPlayer.RoomSession;
            var essHolder = roomSession.EssenceHolder;

            if (essHolder != null)
            {
                var coord = roomSession.EssencePosition;
                sessionPlayer.Post(new GCRespawn(0, 3, 5));
                sessionPlayer.Post(new GCHitEssence(sessionPlayer.Player.PlayerId, essHolder.Player.PlayerId, 2, coord.X, coord.Y, coord.Z, 0, 5));
            }
        }

        public override void OnPlayerKill(RoomSessionPlayer killer, RoomSessionPlayer target, Weapon weapon, byte hitLocation)
        {
            var roomSession = killer.RoomSession;
            var essenceHolder = roomSession.EssenceHolder;

            // check if he died
            if(essenceHolder != null)
            {
                var essenceTargetDied = target == essenceHolder;
                if(essenceTargetDied)
                {
                    var players = roomSession.GetPlayingPlayers();
                    var pos = essenceHolder.Position;

                    roomSession.EssenceHolder = null;

                    foreach (var p in players)
                        p.Post(new GCHitEssence(p.Player.PlayerId, essenceHolder.Player.PlayerId, 3, pos.X, pos.Y, pos.Z, 0, 6));
                }    
            }

            base.OnPlayerKill(killer, target, weapon, hitLocation);
        }
    }
}
