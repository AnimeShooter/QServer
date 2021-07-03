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

        //public override void OnApply(Room room)
        //{
        //    room.IsPointsGame = false;
        //    room.ScoreTime = 10;
        //}

        //public override void tick(RoomSession roomSession)
        //{
        //    // TODO
        //    var essHolder = roomSession.EssemceHolder();
        //    if (!essHolder)
        //        essHolder.AddScore();

        //    var EligiblePoints = roomSession.TimeLeft().TotalSeconds; // timespan
        //    var TeamBluePoints = roomSession.TeamBluePoints;
        //    var TeamYellowPoints = roomSession.TeamYellowPoints;

        //    // check early finish
        //    if ((TeamBluePoints + EligiblePoints < TeamYellowPoints) || (TeamYellowPoints + EligiblePoints < TeamBluePoints))
        //        roomSession.Finish();

        //    if (!roomSession.EssenceReset && roomSession.EssenceDropped)
        //        if (roomSession.ElapsedEssenceDropTime > 20) // Essence Reset Timer
        //            roomSession.ResetEssence(); // reset position
        //}

        //public override void OnPlayerSync(RoomSessionPlayer sessionPlayer)
        //{
        //    var roomSession = sessionPlayer.RoomSession;
        //    var essHolder = roomSession.EssenceHolder;

        //    if(essHolder != null)
        //    {
        //        var coord = roomSession.EssenceCoord;
        //        sessionPlayer.Post(new GCRespawm(0, 3, 5));
        //        sessionPlayer.Post(new GCHitEssence(sessionPlayer.Player.Id, essHolder.Player.Id, 2, coord.x, coord.y, coord.z, 0, 5));
        //    }
        //}
        // TODO
    }
}
