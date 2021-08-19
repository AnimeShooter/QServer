using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class VIP : GameMode
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
            if (roomSession.GetElapsedBlueVipTime() > 50 && !roomSession.IsNextBlueVipEligible())
                roomSession.FindNextBlueVIP();
            if (roomSession.GetElapsedYellowVipTime() > 50 && !roomSession.IsNextYellowVipEligible())
                roomSession.FindNextYellowVip();
        }

        public override void OnPlayerSync(RoomSessionPlayer sessionPlayer)
        {
            var roomSession = sessionPlayer.RoomSession;
            var blueVip = roomSession.BlueVIP;
            var yellowVip = roomSession.YellowVIP;

            if(blueVip != null)
            {
                var pos = blueVip.Position;
                sessionPlayer.UpdateCoords(pos);
                sessionPlayer.Post(new GCRespawn(blueVip.Player.PlayerId, blueVip.Character, 0, pos.X, pos.Y, pos.Z, true));
                sessionPlayer.Post(new GCGameState(blueVip.Player.PlayerId, 8));
            }

            if(yellowVip != null)
            {
                var pos = yellowVip.Position;
                sessionPlayer.UpdateCoords(pos);
                sessionPlayer.Post(new GCRespawn(yellowVip.Player.PlayerId, yellowVip.Character, 0, pos.X, pos.Y, pos.Z, true));
                sessionPlayer.Post(new GCGameState(yellowVip.Player.PlayerId, 8));
            }

            base.OnPlayerSync(sessionPlayer);
        }

        public override void OnPlayerKill(RoomSessionPlayer killer, RoomSessionPlayer target, Weapon weapon, byte hitLocation)
        {
            var roomSession = killer.RoomSession;
            var blueVip = roomSession.BlueVIP;
            var yellowVip = roomSession.YellowVIP;

            if(target == blueVip)
            {
                if (killer.Team == 2)
                    killer.AddScore();
                else
                    roomSession.AddPointsForTeam(2, 1);
            }else if(target == yellowVip)
            {
                if (killer.Team == 1)
                    killer.AddScore();
                else
                    roomSession.AddPointsForTeam(1, 1);
            }

            base.OnPlayerKill(killer, target, weapon, hitLocation);
        }
    }
}
