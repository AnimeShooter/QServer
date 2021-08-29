using System;
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

        public override void OnStart(RoomSession roomSession)
        {
            //var newNpc = new GCPvENpcInit(0,);

            roomSession.RelayPlaying<GCPvENpcInit>((uint)0, (uint)1, (ushort)1, (byte)1, new Spawn() { X = 0, Y = 0, Z = 0 });

            base.OnStart(roomSession);
        }

        public override void Tick(RoomSession roomSession)
        {

            // GCPvEEndRound
            // GCPvEStarRound

            // Reset timers? (Stage 2 goes by time)


            base.Tick(roomSession);
        }
    }
}
