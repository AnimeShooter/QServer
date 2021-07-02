using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Singleton;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Packets;

namespace Qserver.GameServer.Network.Managers
{
    public class LoginHandler : SingletonBase<LoginHandler>
    {
        public PacketWriter InvalidPassword()
        {
            PacketWriter pw = new PacketWriter(Opcode.AUTH_LOGIN_FAIL);
            pw.WriteInt32(1101);
            return pw;
        }

        public PacketWriter InvalidUsername()
        {
            PacketWriter pw = new PacketWriter(Opcode.AUTH_LOGIN_FAIL);
            pw.WriteInt32(1102);
            return pw;
        }

        public PacketWriter InvalidVersion()
        {
            PacketWriter pw = new PacketWriter(Opcode.AUTH_LOGIN_FAIL);
            pw.WriteInt32(503);
            return pw;
        }

        public PacketWriter LoginSuccess(byte[] uuid, int gameHost)
        {
            PacketWriter pw = new PacketWriter(Opcode.AUTH_LOGIN_RSP);
            pw.WriteInt32(gameHost);
            pw.WriteBytes(uuid);
            return pw;
        }

        public PacketWriter HandshakeResponse(byte[] keyPart)
        {
            PacketWriter pw = new PacketWriter(Opcode.KEY_EXCHANGE_RSP);

            //setCryptoType(BlowfishInstance::CryptoType::PUBLIC);
            //writeArray < char, 4 > (key);
            //writeInt(0);

            return pw;
        }
    }
}
