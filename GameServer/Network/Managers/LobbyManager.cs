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
    public class LobbyManager : SingletonBase<LobbyManager>
    {
        #region Account
        public PacketWriter Authenticated(Player player)
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_LOGIN_RSP, 0x05);
            pw.WriteUInt32(player.PlayerId);
            pw.WriteBytes(new byte[42]);

            //pw.WriteWString(player.Name, 16);
            //pw.WriteBytes(new byte[16]); // 16 char name
            pw.WriteBytes(new byte[16] { 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x00, 0x00 }); // 16 char name

            pw.WriteUInt32(player.StatsManager.PlayTimeInMinutes);
            pw.WriteUInt32(player.Cash);
            pw.WriteUInt32(player.Rank);
            pw.WriteUInt16(0); // unk
            pw.WriteUInt16(player.Character);
            pw.WriteUInt32(player.Level);
            pw.WriteBytes(new byte[8]);

            // player settings
            pw.WriteUInt8(1); // accept PM
            pw.WriteUInt8(1); // accept game invites 
            pw.WriteUInt8(1); // accept friend invites 
            pw.WriteUInt8(1); // unk
            pw.WriteUInt8(1); // accept trade requests
            pw.WriteBytes(new byte[20]);
            pw.WriteUInt8(4); // tutorial status
            pw.WriteBytes(new byte[12]);

            // player containers
            pw.WriteUInt16(200); // max inventory size
            pw.WriteUInt16(50); // max friendlist size
            pw.WriteUInt16(10); // max inc/out friend list size
            pw.WriteUInt16(50); // max memo send count
            pw.WriteUInt16(50); // left over memo send count

            // player stats
            pw.WriteUInt32(1); // ?
            pw.WriteUInt32(player.Experience);
            pw.WriteUInt32(player.Don);
            pw.WriteUInt32(0);
            pw.WriteUInt32(player.StatsManager.Kills);
            pw.WriteUInt32(player.StatsManager.Deaths);
            pw.WriteUInt32(player.StatsManager.NormalWins);
            pw.WriteUInt32(player.StatsManager.NormalLosses);
            pw.WriteUInt32(player.StatsManager.NormalDrews);
            pw.WriteUInt32(0); // unk
            pw.WriteUInt32(player.StatsManager.MissionWins);
            pw.WriteUInt32(player.StatsManager.MissionLosses);
            pw.WriteUInt32(player.StatsManager.MissionDrews);

            pw.WriteBytes(new byte[72]); // unk

            pw.WriteUInt32(player.StatsManager.SlackerPoints);
            pw.WriteUInt32(player.Coins);
            pw.WriteBytes(new byte[44]);

            var characters = player.EquipmentManager.UnlockerCharacters;
            foreach (ushort character in characters)
            {
                ulong[] armor = new ulong[9]; // player.EquipmentManager.GetArmorByCharacter(character); // 9
                ulong[] weapons = new ulong[4]; // player.EquipmentManager.GetWeaponsByCharacter(character); // 4
                
                pw.WriteUInt16(character);
                foreach (ulong armorCardId in armor)
                    pw.WriteUInt64(armorCardId);
                
                foreach(ulong weaponCardId in weapons)
                {
                    pw.WriteUInt64(weaponCardId);
                    pw.WriteUInt64(0); // unk
                }
            }

            pw.WriteBytes(new byte[2455 - (138 * characters.Count)]);


            var achievements = player.AchievementContainer.List;
            pw.WriteUInt16((ushort)achievements.Count);
            foreach(var achievement in achievements)
            {
                pw.WriteUInt32(achievement);
                pw.WriteUInt32(0); // unk
            }

            pw.WriteBytes(new byte[1635 - (achievements.Count * 8)]);

            return pw;
        }

        public PacketWriter Banned()
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_LOGIN_FAIL, 0x05);
            pw.WriteUInt32(819);
            return pw;
        }

        public PacketWriter DuplicateLogin()
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_LOGIN_FAIL, 0x05);
            pw.WriteUInt32(802);
            return pw;
        }

        public PacketWriter VerificationFailure()
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_LOGIN_FAIL, 0x05);
            pw.WriteUInt32(204);
            return pw;
        }

        public PacketWriter UpdateAcount()
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_UPDATE_ACCOUNT, 0x05);

            // TODO

            return pw;
        }



        #endregion Account

        #region Channel
        public PacketWriter ChannelList(List<Channel> channels)
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_CHANNELS_RSP, 0x05);

            ushort len = (ushort)channels.Count;
            pw.WriteUInt16(len);
            pw.WriteUInt16(len);
            pw.WriteUInt16(len);

            foreach(var channel in channels)
            {
                pw.WriteUInt16(channel.Id);

                pw.WriteWString(channel.Name, 31);

                pw.WriteUInt8(channel.MinLevel);
                pw.WriteUInt8(channel.MaxLevel);
                pw.WriteUInt16(channel.CurrPlayers);
                pw.WriteUInt16(channel.MaxPlayers);
                pw.WriteBytes(new byte[51]);
            }

            return pw;
        }

        public PacketWriter ChannelHost(uint id)
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_CHANNEL_CONNECT_RSP, 0x05);

            pw.WriteUInt32(id);
            pw.WriteUInt64(02);
            pw.WriteUInt32(0x0100007F);
            pw.WriteUInt32(01);
            pw.WriteUInt32(04);
            pw.WriteUInt32(03);
            
            return pw;
        }
        #endregion Channel
    }
}
