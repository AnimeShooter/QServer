using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNL.Types;
using TNL.Data;
namespace Qserver.GameServer.Qpang
{
    public static class EventRegister
    {
        public static void RegisterTNLEvents()
        {
            // CG
            new NetClassRepInstance<CGRoom>("GCRoom", (uint)NetClassMask.NetClassGroupGameMask, NetClassType.NetClassTypeEvent, 0);
            new NetClassRepInstance<CGRoomInfo>("CGRoomInfo", (uint)NetClassMask.NetClassGroupGameMask, NetClassType.NetClassTypeEvent, 0);
            new NetClassRepInstance<CGAuth>("CGAuth", (uint)NetClassMask.NetClassGroupGameMask, NetClassType.NetClassTypeEvent, 0);

            // GC
            new NetClassRepInstance<GCRoom>("GCRoom", (uint)NetClassMask.NetClassGroupGameMask, NetClassType.NetClassTypeEvent, 0);
            new NetClassRepInstance<GCCard>("GCCard", (uint)NetClassMask.NetClassGroupGameMask, NetClassType.NetClassTypeEvent, 0);
            new NetClassRepInstance<GCArrangedAccept>("GCArrangedAccept", (uint)NetClassMask.NetClassGroupGameMask, NetClassType.NetClassTypeEvent, 0);
            new NetClassRepInstance<GCArrangedConn>("GCArrangedConn", (uint)NetClassMask.NetClassGroupGameMask, NetClassType.NetClassTypeEvent, 0);
        }
    }
}
