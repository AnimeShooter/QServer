using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Util;
using Qserver.GameServer.Qpang;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Managers;
using Qserver.GameServer.Network.Packets;
using System.Threading;
using BCrypt.Net;

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
            string wUsername = packet.ReadWString(20); // wchar convert!
            string wPassword = packet.ReadWString(16); // wchar convert!
            byte[] skip = packet.ReadBytes(12); // unk
            int version = packet.ReadInt32();

            var user = Game.Instance.UsersRepository.GetUserCredentials(wUsername).Result;
            if(user.password == null)
            {
                Log.Message(LogType.ERROR, $"Failed login attempt for {wUsername}, (invalid username)");
                manager.Send(AuthManager.Instance.InvalidUsername());
                return;
            }
            if (!BCrypt.Net.BCrypt.Verify(wPassword, user.password))
            {
                Log.Message(LogType.ERROR, $"Failed login attempt for {wUsername}, (invalid password)");
                manager.Send(AuthManager.Instance.InvalidPassword());
                return;
            }

            string uuid = Util.Util.GenerateUUID();
            Game.Instance.UsersRepository.UpdateUUID(wUsername, uuid).GetAwaiter();

            Log.Message(LogType.NORMAL, $"Succesful login attempt for {wUsername}");
            manager.Send(AuthManager.Instance.LoginSuccess(Encoding.ASCII.GetBytes(uuid), Settings.SERVER_IP2)); // game host local TODO not hardcode!!!
        }
    }
}
