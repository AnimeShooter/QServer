using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class Practice : GameMode
    {
        public override bool IsMissionMode()
        {
            return false;
        }

        public override bool IsTeamMode()
        {
            return true;
        }

        public override bool IsPractice()
        {
            return true;
        }

        public override void Tick(RoomSession roomSession)
        {

            // TODO: keep track of NPC's

            base.Tick(roomSession);
        }
    }
}
