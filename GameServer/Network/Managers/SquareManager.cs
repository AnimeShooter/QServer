using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Singleton;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Packets;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Network
{
    public class SquareManager : SingletonBase<SquareManager>
    {
        public PacketWriter Login()
        {
            PacketWriter pw = new PacketWriter(Opcode.SQUARE_LOGIN_RSP, 0x05);
           
            return pw;
        }
    }
}
