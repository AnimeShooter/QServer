using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Util;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Managers;
using Qserver.GameServer.Network.Packets;
using System.Threading;

namespace Qserver.GameServer.Network.Handlers
{
    public class AuthHandler
    {
        
        public static void HandleHandshake(PacketReader packet, ServerManager manager)
        {
            manager.Send(LoginHandler.Instance.HandshakeResponse(manager.KeyPart));
        }

        public static void HandleLoginRequest(PacketReader packet, ServerManager manager)
        {
            packet.ReadBytes(16); // unk
            byte[] wUsername = packet.ReadBytes(20); // wchar convert!
            byte[] wPassword = packet.ReadBytes(16); // wchar convert!
            packet.ReadBytes(12); // unk
            int version = packet.ReadInt32();

            // TODO: revision

            if (false)
                manager.Send(LoginHandler.Instance.InvalidVersion());

            // TODO: database check

            LoginHandler.Instance.LoginSuccess(new byte[4], 0x7F000001); // game host local
        }
    }
}
