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
         * Objects:
         * stag1:
         * - 1 small fence
         * - 2 big fence
         * - 3 Red plafroem
         * - 4 Red platform 2
         * - 5 button
         * 
         * Stage2:
         * - 6 Essence (0, 0, 0)
         */

        public override void OnStart(RoomSession roomSession)
        {
            roomSession.PvEEntityManager.SetupStage(1);
            
            base.OnStart(roomSession);
        }

        public override void OnPlayerSync(RoomSessionPlayer session)
        {
            session.RoomSession.PvEEntityManager.SyncPlayer(session);
            base.OnPlayerSync(session);
        }

        public override void Tick(RoomSession roomSession)
        {

            // Reset timers? (Stage 2 goes by time)

            base.Tick(roomSession);
        }
    }
}
