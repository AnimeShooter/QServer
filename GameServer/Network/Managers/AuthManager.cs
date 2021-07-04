using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Singleton;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Packets;

namespace Qserver.GameServer.Network.Managers
{
    public class AuthManager : SingletonBase<AuthManager>
    {
        public PacketWriter InvalidPassword()
        {
            PacketWriter pw = new PacketWriter(Opcode.AUTH_LOGIN_FAIL, 0x05);
            pw.WriteInt32(1101);
            return pw;
        }

        public PacketWriter InvalidUsername()
        {
            PacketWriter pw = new PacketWriter(Opcode.AUTH_LOGIN_FAIL, 0x05);
            pw.WriteInt32(1102);
            return pw;
        }

        public PacketWriter InvalidVersion()
        {
            PacketWriter pw = new PacketWriter(Opcode.AUTH_LOGIN_FAIL, 0x05);
            pw.WriteInt32(503);
            return pw;
        }

        public PacketWriter LoginSuccess(byte[] uuid, int gameHost)
        {
            PacketWriter pw = new PacketWriter(Opcode.AUTH_LOGIN_RSP, 0x05);
            pw.WriteInt32(gameHost);
            pw.WriteBytes(uuid);
            return pw;
        }

        public PacketWriter HandshakeResponse(byte[] keyPart)
        {
            PacketWriter pw = new PacketWriter(Opcode.KEY_EXCHANGE_RSP); // ServerPacket but no key yet (so 0x01)
            byte[] clientKey = keyPart.Take(4).ToArray();
            clientKey[0] -= 7; // client knows to add
            pw.WriteBytes(clientKey);
            pw.WriteUInt32(0);
            return pw;
        }
    }
}
