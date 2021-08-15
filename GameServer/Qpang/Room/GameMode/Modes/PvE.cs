using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class PvE : GameMode
    {
        public override bool IsMissionMode()
        {
            return true;
        }

        public override bool IsTeamMode()
        {
            return true;
        }

        // TODO
        /*
         *  Not friendly fire
         *  
         *  charge (ask) coin on revive
         *  
         *  ...
         * 
         */
    }
}
