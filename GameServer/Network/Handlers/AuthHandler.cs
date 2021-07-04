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
        
        public static void HandleHandshake(PacketReader packet, ConnServer manager)
        {
            manager.Encryption = 0x01; // Public
            manager.KeyPart = new byte[] { 0x07, 0x00, 0x00, 0x00, 0x29, 0xA1, 0xD3, 0x56 }; // TODO: randomize
            manager.Send(AuthManager.Instance.HandshakeResponse(manager.KeyPart));
        }

        public static void HandleLoginRequest(PacketReader packet, ConnServer manager)
        {
            packet.ReadBytes(16); // unk
            byte[] wUsername = packet.ReadBytes(20); // wchar convert!
            byte[] wPassword = packet.ReadBytes(16); // wchar convert!
            packet.ReadBytes(12); // unk
            int version = packet.ReadInt32();

            // TODO: revision

            //if (true)
            //{
            //    manager.Send(LoginHandler.Instance.InvalidUsername());
            //    return;
            //}
                

            // TODO: database check

            manager.Send(AuthManager.Instance.LoginSuccess(new byte[16] {0x7F, 0x00, 0x00, 0x01, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F }, 0x0100007F)); // game host local
        }
    }
}
