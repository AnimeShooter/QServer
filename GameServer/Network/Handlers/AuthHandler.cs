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

            var pw = Game.Instance.PlayerRepository.GetUserPassword(wUsername).Result;
            string hashedPw = BCrypt.Net.BCrypt.HashPassword(pw);
            if (!BCrypt.Net.BCrypt.Verify(wPassword, pw))
            {
                manager.Send(AuthManager.Instance.InvalidUsername());
                return;
            }

            Random rnd = new Random();
            string part1 = "";
            for(int i = 0; i < 5; i++)
                part1 += (char)rnd.Next(0x41, 0x5B);

            string uuid = $"{part1}-{rnd.Next(1000,9999)}-{rnd.Next(1000,0xFFFF).ToString("X4")}";

            Game.Instance.PlayerRepository.UpdateUUID(wUsername, uuid).GetAwaiter();

            uint IP = 0x0100007F;
            string[] ipSplit = Settings.SERVER_IP.Split('.');
            if (ipSplit.Length == 4)
                IP = (uint)((Convert.ToByte(ipSplit[3]) * 0x1_00_00_00) + (Convert.ToByte(ipSplit[2]) * 0x1_00_00) + (Convert.ToByte(ipSplit[1]) * 0x1_00) + (Convert.ToByte(ipSplit[0])));

            manager.Send(AuthManager.Instance.LoginSuccess(Encoding.ASCII.GetBytes(uuid), IP)); // game host local
        }
    }
}
