﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class PvE : GameMode
    {
        public override bool IsPvE()
        {
            return true;
        }

        // TODO
        /*
         *  No friendly fire
         *  
         *  charge (ask) coin on revive
         *  
         *  tick -> move enemy's
         *  
         *  ...
         * 
         */
        public override void Tick(RoomSession roomSession)
        {

            // GCPvEEndRound
            // GCPvEStarRound

            // Reset timers? (Stage 2 goes by time)


            base.Tick(roomSession);
        }
    }
}
