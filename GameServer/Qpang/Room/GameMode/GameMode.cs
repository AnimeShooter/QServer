using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class GameMode
    {
        public enum Mode
        {
            DM = 1,
            TDM = 2,
            PTE = 3,
            VIP = 4,
            PRACTICE = 5,
            PREY = 8,
            PVE = 9,
        };

        public virtual bool IsTeamMode()
        {
            return false;
        }


        public virtual bool IsMissionMode()
        {
            return false;
        }


        //public virtual void Tick(RoomSession roomSession) { }

        //public virtual void OnApply(Room room) { }

        //public virtual void OnStart(RoomSession roomSession) { }

        //public virtual void OnPlayerSync(RoomSessionPlayer session) { }

        //public virtual void OnPlayerKill(RoomSessionPlayer Killer, RoomSessionPlayer target, Weapon weapon, byte hitLocation) { }

    }
}
