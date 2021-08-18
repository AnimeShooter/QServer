using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class AmmoClip : GameItem
    {
        public override uint OnPickUp(RoomSessionPlayer session)
        {
            session.WeaponManager.RefillCurrentWeapon();
            return 0;
        }
    }
}
