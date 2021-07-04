using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Singleton;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Packets;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Network.Managers
{
    public class ParkManager : SingletonBase<ParkManager>
    {
        public PacketWriter Authenticated(Player player)
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_LOGIN_RSP, 0x05);
            pw.WriteUInt32(player.PlayerId);
            pw.WriteBytes(new byte[42]);

            pw.WriteBytes(new byte[16]); // 16 char name
            //pw.WriteString(player.na) // 16 char name

            //pw.WriteUInt32(player.)

            return pw;
        }

        public PacketWriter Banned()
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_LOGIN_FAIL, 0x05);
            pw.WriteUInt32(819);
            return pw;
        }

    }
}
