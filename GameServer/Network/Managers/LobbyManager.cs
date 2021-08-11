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
            pw.WriteBytes(new byte[42]); // OG 42
            pw.WriteWString(player.Name, 16);
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
            pw.WriteUInt8(4); // tutorial status (4)
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
                ulong[] armor = player.EquipmentManager.GetArmorByCharacter(character); // 9
                ulong[] weapons = player.EquipmentManager.GetWeaponsByCharacter(character); // 4
                
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

            pw.WriteBytes(new byte[1635 - achievements.Count * 8]);

            return pw;
        }
        public PacketWriter Banned()
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_LOGIN_FAIL);
            pw.WriteUInt32(819);
            return pw;
        }
        public PacketWriter DuplicateLogin()
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_LOGIN_FAIL);
            pw.WriteUInt32(802);
            return pw;
        }
        public PacketWriter VerificationFailure()
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_LOGIN_FAIL);
            pw.WriteUInt32(204);
            return pw;
        }
        public PacketWriter UpdateAccount(Player player)
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_UPDATE_ACCOUNT);
            pw.WriteUInt32(player.PlayerId);
            pw.WriteBytes(new byte[42]); // OG 42
            pw.WriteWString(player.Name, 16);
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
            pw.WriteUInt8(4); // tutorial status (4)
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
                ulong[] armor = player.EquipmentManager.GetArmorByCharacter(character); // 9
                ulong[] weapons = player.EquipmentManager.GetWeaponsByCharacter(character); // 4

                pw.WriteUInt16(character);
                foreach (ulong armorCardId in armor)
                    pw.WriteUInt64(armorCardId);

                foreach (ulong weaponCardId in weapons)
                {
                    pw.WriteUInt64(weaponCardId);
                    pw.WriteUInt64(0); // unk
                }
            }

            pw.WriteBytes(new byte[2455 - (138 * characters.Count)]);


            var achievements = player.AchievementContainer.List;
            pw.WriteUInt16((ushort)achievements.Count);
            foreach (var achievement in achievements)
            {
                pw.WriteUInt32(achievement);
                pw.WriteUInt32(0); // unk
            }

            pw.WriteBytes(new byte[1635 - achievements.Count * 8]);

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

                pw.WriteWString(channel.Name, 30);

                pw.WriteUInt8(channel.MinLevel);
                pw.WriteUInt8(channel.MaxLevel);
                pw.WriteUInt16(channel.CurrPlayers);
                pw.WriteUInt16(channel.MaxPlayers);
                pw.WriteBytes(new byte[51]); // OG 51

                pw.WriteBytes(new byte[1]); // idk ?
            }

            return pw;
        }
        public PacketWriter ChannelHost(Channel channel)
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_CHANNEL_CONNECT_RSP, 0x05);

            pw.WriteUInt32(channel.Id);
            pw.WriteUInt64(00);

            string[] ipSplit = channel.IP.Split('.');
            if(ipSplit.Length != 4)
                pw.WriteUInt32(0x0100007F);
            else
                for (int i = 0; i < 4; i++)
                    pw.WriteUInt8(Convert.ToByte(ipSplit[i]));

            pw.WriteUInt32(00);
            pw.WriteUInt32(00);
            pw.WriteUInt32(00);
            
            return pw;
        }
        #endregion Channel

        #region Equipment
        public PacketWriter EquippedSkillCards(InventoryCard[] skillCards)
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_EQUIPPED_SKILS_RSP);
            for(int i = 0; i < 5; i++)
            {
                foreach(var card in skillCards)
                {
                    // InventoryCard
                    pw.WriteUInt64(card.Id);        // 0 
                    pw.WriteUInt32(card.ItemId);    // 8
                    pw.WriteUInt8(10);              // 12
                    pw.WriteUInt8(card.Type);       // 13
                    pw.WriteUInt8(0);               // 14
                    pw.WriteUInt8(card.IsGiftable ? (byte)1 : (byte)0); // 15
                    pw.WriteBytes(new byte[6]);     // 16
                    pw.WriteUInt32(card.TimeCreated);   // 22 
                    pw.WriteUInt8(card.IsOpened ? (byte)1 : (byte)0); // 26
                    pw.WriteUInt16(card.IsActive ? (ushort)0 : (ushort)1); // 27
                    pw.WriteUInt8(0);         // 28 hidden
                    pw.WriteUInt8(0);         // 29
                    pw.WriteUInt16(card.Period);    //
                    pw.WriteUInt8(card.PeriodeType);
                    pw.WriteUInt8(0);
                    pw.WriteUInt16(card.BoostLevel);
                    pw.WriteUInt8(card.BoostLevel > 0 ? (byte)1 : (byte)0);
                    pw.WriteUInt8(0);
                    pw.WriteBytes(new byte[4]);
                }
            }
            return pw;
        }
        public PacketWriter SetArmor(ushort characterOffset, ulong[] armor)
        {
            PacketWriter pw = new PacketWriter((Opcode)621);
            pw.WriteUInt16(characterOffset);
            foreach (var piece in armor)
                pw.WriteUInt64(piece);
            return pw;
        }
        public PacketWriter SetWeapons(ushort characterOffset, ulong[] weapons)
        {
            PacketWriter pw = new PacketWriter((Opcode)624);
            pw.WriteUInt16(characterOffset);
            foreach (var weapon in weapons)
            {
                pw.WriteUInt64(weapon);
                pw.WriteUInt64(0);
            } 
            return pw;
        }
        #endregion

        #region Friend
        public PacketWriter AcceptIncommingFriend(Friend friend)
        {
            PacketWriter pw = new PacketWriter((Opcode)702);
            // Friend
            pw.WriteUInt32(friend.FriendId);
            pw.WriteBytes(new byte[4]);
            pw.WriteUInt8(friend.State);
            pw.WriteUInt8(friend.IsOnline ? (byte)1 : (byte)0);
            pw.WriteUInt16(friend.Level);
            pw.WriteWString(friend.Nickname, 16);
            return pw;
        }
        public PacketWriter AddIncommingFriend(Friend friend)
        {
            PacketWriter pw = new PacketWriter((Opcode)700);
            // Friend
            pw.WriteUInt32(friend.FriendId);
            pw.WriteBytes(new byte[4]);
            pw.WriteUInt8(friend.State);
            pw.WriteUInt8(friend.IsOnline ? (byte)1 : (byte)0);
            pw.WriteUInt16(friend.Level);
            pw.WriteWString(friend.Nickname, 16);
            return pw;
        }
        public PacketWriter AddOutgoingFriend(Friend friend)
        {
            PacketWriter pw = new PacketWriter((Opcode)698);
            // Friend
            pw.WriteUInt32(friend.FriendId);
            pw.WriteBytes(new byte[4]);
            pw.WriteUInt8(friend.State);
            pw.WriteUInt8(friend.IsOnline ? (byte)1 : (byte)0);
            pw.WriteUInt16(friend.Level);
            pw.WriteWString(friend.Nickname, 16);
            return pw;
        }
        public PacketWriter AppearOffline(uint playerId)
        {
            PacketWriter pw = new PacketWriter((Opcode)607);
            pw.WriteUInt32(playerId);
            return pw;
        }
        public PacketWriter AppearOnline(uint playerId)
        {
            PacketWriter pw = new PacketWriter((Opcode)603);
            pw.WriteUInt32(playerId);
            return pw;
        }
        public PacketWriter CancelOutgoingFriend(uint playerId)
        {
            PacketWriter pw = new PacketWriter((Opcode)710);
            pw.WriteUInt32(playerId);
            return pw;
        }
        public PacketWriter DenyIncomingFriend(uint playerId)
        {
            PacketWriter pw = new PacketWriter((Opcode)706);
            pw.WriteUInt32(playerId);
            return pw;
        }
        public PacketWriter FriendList(List<Friend> friends)
        {
            PacketWriter pw = new PacketWriter((Opcode)695);

            ushort len = (ushort)friends.Count;
            pw.WriteUInt16(len);
            pw.WriteUInt16(len);
            pw.WriteUInt16(len);

            foreach(var f in friends)
            {
                // Friend
                pw.WriteUInt32(f.FriendId);
                pw.WriteBytes(new byte[4]);
                pw.WriteUInt8(f.State);
                pw.WriteUInt8(f.IsOnline ? (byte)1 : (byte)0);
                pw.WriteUInt16(f.Level);
                pw.WriteWString(f.Nickname, 16);
            }
            return pw;
        }
        public PacketWriter FriendRemoved(Friend friend)
        {
            PacketWriter pw = new PacketWriter((Opcode)716);
            pw.WriteUInt32(friend.FriendId);
            pw.WriteBytes(new byte[8]);
            pw.WriteWString(friend.Nickname, 16);
            return pw;
        }
        public PacketWriter IncomingFriendCancelled(Player friend)
        {
            PacketWriter pw = new PacketWriter((Opcode)712);
            pw.WriteUInt32(friend.PlayerId);
            pw.WriteBytes(new byte[8]);
            pw.WriteWString(friend.Name, 16);
            return pw;
        }
        public PacketWriter OutgoingFriendAccepted(Friend friend)
        {
            PacketWriter pw = new PacketWriter((Opcode)704);
            // Friend
            pw.WriteUInt32(friend.FriendId);
            pw.WriteBytes(new byte[4]);
            pw.WriteUInt8(friend.State);
            pw.WriteUInt8(friend.IsOnline ? (byte)1 : (byte)0);
            pw.WriteUInt16(friend.Level);
            pw.WriteWString(friend.Nickname, 16);
            return pw;
        }
        public PacketWriter OutgoingFriendCancelled(Player player)
        {
            PacketWriter pw = new PacketWriter((Opcode)708);
            pw.WriteUInt32(player.PlayerId);
            pw.WriteBytes(new byte[8]);
            pw.WriteWString(player.Name, 16);
            return pw;
        }
        public PacketWriter RemoveFriend(uint friendId)
        {
            PacketWriter pw = new PacketWriter((Opcode)714);
            pw.WriteUInt32(friendId);
            return pw;
        }
        #endregion

        #region Gameroom
        public PacketWriter RoomList(List<Room> rooms)
        {
            PacketWriter pw = new PacketWriter((Opcode)759);

            ushort size = (ushort)rooms.Count;

            pw.WriteUInt16(size);
            pw.WriteUInt16(size);
            pw.WriteUInt16(size);

            foreach(var room in rooms)
            {
                // Room
                pw.WriteUInt32(room.Host);
                pw.WriteUInt16(room.Port);
                pw.WriteUInt32(room.Id);
                pw.WriteBytes(new byte[2]);
                pw.WriteWString(room.Name, 30); 
                pw.WriteBytes(new byte[14]);
                pw.WriteUInt8(room.Map);
                pw.WriteUInt8((byte)room.Mode);
                pw.WriteUInt8(8); // pw
                pw.WriteUInt8(room.State);
                pw.WriteUInt8(room.PlayerCount);
                pw.WriteUInt8(room.MaxPlayers);
                pw.WriteBytes(new byte[5]);
                pw.WriteUInt8(room.LevelLimited ? (byte)1 : (byte)0);
                pw.WriteUInt8(room.TeamSorting ? (byte)1 : (byte)0);
                pw.WriteUInt8(room.SkillsEnabled ? (byte)1 : (byte)0);
                pw.WriteBytes(new byte[2]);
                pw.WriteUInt8(room.MeleeOnly ? (byte)1 : (byte)0);
            }
            return pw;
        }
        public PacketWriter UpdateGameSettings(uint host, ushort port, bool isEnabled)
        {
            PacketWriter pw = new PacketWriter((Opcode)770);
            pw.WriteUInt8(isEnabled ? (byte)1 : (byte)0);
            pw.WriteUInt32(host);
            pw.WriteUInt16(port);
            pw.WriteBytes(new byte[42]);
            return pw;
        }
        #endregion

        #region Trade
        public PacketWriter TradeResponse(uint playerId)
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_TRADE_RSP);
            pw.WriteUInt32(0); // unk 1
            pw.WriteUInt32(0); // unk 2
            pw.WriteUInt32(0); // unk 3
            return pw;
        }
        public PacketWriter SendTradeRequest(uint playerId)
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_TRADE_ACT);
            pw.WriteUInt32(playerId); //test
            return pw;
        }
        #endregion

        #region Inventory
        public PacketWriter CardExtended(InventoryCard card, uint balance, bool isCash)
        {
            PacketWriter pw = new PacketWriter((Opcode)810);
            pw.WriteUInt8(isCash ? (byte)1 : (byte)0);
            pw.WriteUInt32(balance);
            pw.WriteUInt32(0);
            pw.WriteUInt8(1);
            return pw;
        }
        public PacketWriter DisableFunctionCard(InventoryCard card)
        {
            PacketWriter pw = new PacketWriter((Opcode)862);

            // InventoryCard
            pw.WriteUInt64(card.Id); // 0
            pw.WriteUInt32(card.ItemId); // 8
            pw.WriteUInt8(10); // 12
            pw.WriteUInt8(card.Type); // 13
            pw.WriteUInt8(0); // 14
            pw.WriteUInt8(card.IsGiftable ? (byte)1 : (byte)0); // 15
            pw.WriteBytes(new byte[6]); // 16
            pw.WriteUInt32(card.TimeCreated);
            pw.WriteUInt8(card.IsOpened ? (byte)1 : (byte)0); // 26
            pw.WriteUInt16(card.IsActive ? (ushort)0 : (ushort)1); // 27
            pw.WriteUInt8(0); // 28; hidden
            pw.WriteUInt8(0); // 29
            pw.WriteUInt16(card.Period); // 31
            pw.WriteUInt8(card.PeriodeType); // 33
            pw.WriteUInt8(0); // 34
            pw.WriteUInt16(card.BoostLevel); // 35
            pw.WriteUInt8(card.BoostLevel > 0 ? (byte)1 : (byte)0); // 37
            pw.WriteUInt8(0); //  38
            pw.WriteBytes(new byte[4]); // 39

            return pw;
        }
        public PacketWriter EnabledFunctionCard(InventoryCard card)
        {
            PacketWriter pw = new PacketWriter((Opcode)835);

            // InventoryCard
            pw.WriteUInt64(card.Id); // 0
            pw.WriteUInt32(card.ItemId); // 8
            pw.WriteUInt8(10); // 12
            pw.WriteUInt8(card.Type); // 13
            pw.WriteUInt8(0); // 14
            pw.WriteUInt8(card.IsGiftable ? (byte)1 : (byte)0); // 15
            pw.WriteBytes(new byte[6]); // 16 
            pw.WriteUInt32(card.TimeCreated);
            pw.WriteUInt8(card.IsOpened ? (byte)1 : (byte)0); // 26
            pw.WriteUInt16(card.IsActive ? (ushort)0 : (ushort)1); // 27
            pw.WriteUInt8(0); // 28; hidden
            pw.WriteUInt8(0); // 29
            pw.WriteUInt16(card.Period); // 31
            pw.WriteUInt8(card.PeriodeType); // 33
            pw.WriteUInt8(0); // 34
            pw.WriteUInt16(card.BoostLevel); // 35
            pw.WriteUInt8(card.BoostLevel > 0 ? (byte)1  : (byte)0); // 37
            pw.WriteUInt8(0); //  38
            pw.WriteBytes(new byte[4]); // 39

            return pw;
        }
        public PacketWriter GiftCardSuccess(ulong cardId)
        {
            PacketWriter pw = new PacketWriter((Opcode)813);
            pw.WriteBytes(new byte[34]);
            pw.WriteUInt64(cardId);
            return pw;
        }
        public PacketWriter Gifts(List<InventoryCard> gifts)
        {
            PacketWriter pw = new PacketWriter((Opcode)746);
            ushort size = (ushort)gifts.Count;
            pw.WriteUInt16(size);
            pw.WriteUInt16(size);
            pw.WriteUInt16(size);
            foreach(var gift in gifts)
            {
                pw.WriteWString("", 16); // idk
                pw.WriteUInt64(gift.Id);
                pw.WriteUInt8(1);
                pw.WriteUInt32(gift.TimeCreated);
            }
            return pw;
        }
        public PacketWriter Inventory(List<InventoryCard> cards)
        {
            PacketWriter pw = new PacketWriter((Opcode)781);
            ushort len = (ushort)cards.Count;
            pw.WriteUInt16(len);
            pw.WriteUInt16(len);
            pw.WriteUInt16(len);
            foreach(var card in cards)
            {
                // InventoryCard
                pw.WriteUInt64(card.Id); // 0
                pw.WriteUInt32(card.ItemId); // 8
                pw.WriteUInt8(10); // 12
                pw.WriteUInt8(card.Type); // 13
                pw.WriteUInt8(0); // 14
                pw.WriteUInt8(card.IsGiftable ? (byte)1 : (byte)0); // 15
                pw.WriteBytes(new byte[6]); // 16
                pw.WriteUInt32(card.TimeCreated); // 22
                pw.WriteUInt8(card.IsOpened ? (byte)1 : (byte)0); // 26
                pw.WriteUInt16(card.IsActive ? (ushort)0 : (ushort)1); // 27
                pw.WriteUInt8(0); // 28; hidden
                pw.WriteUInt8(0); // 29
                pw.WriteUInt16(card.Period); // 31
                pw.WriteUInt8(card.PeriodeType); // 33
                pw.WriteUInt8(0); // 34
                pw.WriteUInt16(card.BoostLevel); // 35
                pw.WriteUInt8(card.BoostLevel > 0 ? (byte)1 : (byte)0); // 37
                pw.WriteUInt8(0); //  38
                pw.WriteBytes(new byte[4]); // 39
            }
            return pw;
        }
        public PacketWriter OpenGift(InventoryCard card)
        {
            PacketWriter pw = new PacketWriter((Opcode)743);
            pw.WriteUInt32(0);
            pw.WriteUInt32(0);
            pw.WriteUInt32(1); // card count

            // InventoryCard
            pw.WriteUInt64(card.Id); // 0
            pw.WriteUInt32(card.ItemId); // 8
            pw.WriteUInt8(10); // 12
            pw.WriteUInt8(card.Type); // 13
            pw.WriteUInt8(0); // 14
            pw.WriteUInt8(card.IsGiftable ? (byte)1 : (byte)0); // 15
            pw.WriteBytes(new byte[6]); // 16
            pw.WriteUInt32(card.TimeCreated);
            pw.WriteUInt8(card.IsOpened ? (byte)1 : (byte)0); // 26
            pw.WriteUInt16(card.IsActive ? (ushort)0 : (ushort)1); // 27
            pw.WriteUInt8(0); // 28; hidden
            pw.WriteUInt8(0); // 29
            pw.WriteUInt16(card.Period); // 31
            pw.WriteUInt8(card.PeriodeType); // 33
            pw.WriteUInt8(0); // 34
            pw.WriteUInt16(card.BoostLevel); // 35
            pw.WriteUInt8(card.BoostLevel > 0 ? (byte)1 : (byte)0); // 37
            pw.WriteUInt8(0); //  38
            pw.WriteBytes(new byte[4]); // 39

            return pw;
        }
        public PacketWriter OpenGiftSuccess(Player player, InventoryCard card)
        {
            PacketWriter pw = new PacketWriter((Opcode)743);
            pw.WriteUInt32((uint)card.Id);
            pw.WriteUInt32(player.Don);
            pw.WriteUInt8(1); // card count

            // InventoryCard
            pw.WriteUInt64(card.Id); // 0
            pw.WriteUInt32(card.ItemId); // 8
            pw.WriteUInt8(10); // 12
            pw.WriteUInt8(card.Type); // 13
            pw.WriteUInt8(0); // 14
            pw.WriteUInt8(card.IsGiftable ? (byte)1 : (byte)0); // 15
            pw.WriteBytes(new byte[6]); // 16
            pw.WriteUInt32(card.TimeCreated);
            pw.WriteUInt8(card.IsOpened ? (byte)1 : (byte)0); // 26
            pw.WriteUInt16(card.IsActive ? (ushort)0 : (ushort)1); // 27
            pw.WriteUInt8(0); // 28; hidden
            pw.WriteUInt8(0); // 29
            pw.WriteUInt16(card.Period); // 31
            pw.WriteUInt8(card.PeriodeType); // 33
            pw.WriteUInt8(0); // 34
            pw.WriteUInt16(card.BoostLevel); // 35
            pw.WriteUInt8(card.BoostLevel > 0 ? (byte)1 : (byte)0); // 37
            pw.WriteUInt8(0); //  38
            pw.WriteBytes(new byte[4]); // 39

            return pw;
        }
        public PacketWriter ReceiveGift(InventoryCard card, string sender)
        {
            PacketWriter pw = new PacketWriter((Opcode)821);
            pw.WriteWString(sender, 16);
            pw.WriteUInt64(card.Id);
            pw.WriteUInt8(1);
            pw.WriteUInt64(card.TimeCreated);
            return pw;
        }
        public PacketWriter RemoveCard(ulong cardId)
        {
            PacketWriter pw = new PacketWriter((Opcode)653);
            pw.WriteUInt64(cardId);
            return pw;
        }
        #endregion

        #region Memo
        public PacketWriter Memos(List<Memo> memos)
        {
            PacketWriter pw = new PacketWriter((Opcode)726);
            ushort len = (ushort)memos.Count;

            pw.WriteUInt16(len);
            pw.WriteUInt16(len);
            pw.WriteUInt16(len);

            foreach(var memo in memos)
            {
                pw.WriteUInt64(memo.Id);
                pw.WriteUInt32(memo.SenderId);
                pw.WriteUInt32(memo.Created);
                pw.WriteWString(memo.Nickname, 16);
                pw.WriteWString(memo.Message, 100);
                pw.WriteUInt8(memo.IsOpened ? (byte)1 : (byte)0);
            }

            return pw;
        }
        public PacketWriter SendMemo(Memo memo)
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_SEND_MEMO);
            // TODO
            return pw;
        }
        public PacketWriter DeleteMemo(Memo memo)
        {
            PacketWriter pw = new PacketWriter(Opcode.LOBBY_DELETE_MEMO);
            // TODO
            return pw;
        }
        #endregion

        #region Misc
        public PacketWriter UseCrainFail(ushort character)
        {
            PacketWriter pw = new PacketWriter((Opcode)899);
            pw.WriteUInt16(character);
            return pw;
        }
        public PacketWriter UseCrainSuccess(Player player, List<InventoryCard> cards)
        {
            PacketWriter pw = new PacketWriter((Opcode)898);
            pw.WriteUInt16((ushort)cards.Count);
            pw.WriteUInt32(player.Don);
            pw.WriteUInt32(player.Coins);

            foreach(var card in cards)
            {
                pw.WriteUInt32(card.ItemId);
                pw.WriteBytes(new byte[5]);
                // InventoryCard
                pw.WriteUInt64(card.Id);        // 0 
                pw.WriteUInt32(card.ItemId);    // 8
                pw.WriteUInt8(10);              // 12
                pw.WriteUInt8(card.Type);       // 13
                pw.WriteUInt8(0);               // 14
                pw.WriteUInt8(card.IsGiftable ? (byte)1 : (byte)0); // 15
                pw.WriteBytes(new byte[6]);     // 16
                pw.WriteUInt32(card.TimeCreated);   // 22
                pw.WriteUInt8(card.IsOpened ? (byte)1 : (byte)0); // 26
                pw.WriteUInt16(card.IsActive ? (ushort)0 : (ushort)1); // 27
                pw.WriteUInt8(0);         // 28 hidden
                pw.WriteUInt8(0);         // 29
                pw.WriteUInt16(card.Period);    //
                pw.WriteUInt8(card.PeriodeType);
                pw.WriteUInt8(0);
                pw.WriteUInt16(card.BoostLevel);
                pw.WriteUInt8(card.BoostLevel > 0 ? (byte)1 : (byte)0);
                pw.WriteUInt8(0);
                pw.WriteBytes(new byte[4]);
            }

            return pw;
        }
        public PacketWriter CouponSuccess(uint newDon, uint newCash) // lobby code success
        {
            PacketWriter pw = new PacketWriter((Opcode)852);
            pw.WriteUInt32(0); // 8 unk1
            pw.WriteUInt32(0); // C unk2
            pw.WriteUInt32(newDon); // 10 NewDon
            pw.WriteUInt32(newCash); // 14 NewCash

            byte count = 0; // max 1? 
            pw.WriteUInt8(count); // 18 count
            pw.WriteUInt8(count); // 19 unk src
            for (int i = 0; i < count; i++)
            {
                // unk 0x2B size
            }
            return pw;
        }
        public PacketWriter CouponInvalid() // Lobby Code Invalid
        {
            PacketWriter pw = new PacketWriter((Opcode)853);
            return pw;
        }
        public PacketWriter Broadcast(string message)
        {
            PacketWriter pw = new PacketWriter((Opcode)4);
            pw.WriteUInt32(0);
            pw.WriteBytes(new byte[34]);
            pw.WriteWString(message, 254);
            return pw;
        }
        #endregion

        #region Player
        public PacketWriter PlayerInfoInspector(Player player)
        {
            PacketWriter pw = new PacketWriter((Opcode)692);

            pw.WriteUInt32(player.PlayerId);
            pw.WriteBytes(new byte[42]); // OG 42
            pw.WriteWString(player.Name, 16);
            pw.WriteUInt32(player.StatsManager.PlayTimeInMinutes);
            pw.WriteBytes(new byte[4]);
            pw.WriteUInt8((byte)player.Level);
            pw.WriteUInt8((byte)player.Rank);
            pw.WriteUInt16(player.Character);

            var armor = player.EquipmentManager.GetArmorItemIdsByCharacter(player.Character);
            foreach (var armorId in armor)
                pw.WriteUInt32(armorId);

            var weapons = player.EquipmentManager.GetWeaponItemIdsByCharacter(player.Character);
            foreach(var weaponId in weapons)
            {
                pw.WriteUInt32(weaponId);
                pw.WriteUInt32(0);
            }

            pw.WriteBytes(new byte[40]);
            pw.WriteUInt32(player.Experience);
            pw.WriteUInt32(1);
            pw.WriteUInt32(player.StatsManager.Kills);
            pw.WriteUInt32(player.StatsManager.Deaths);
            pw.WriteUInt32(player.StatsManager.NormalWins);
            pw.WriteUInt32(player.StatsManager.NormalLosses);
            pw.WriteUInt32(player.StatsManager.NormalDrews);
            pw.WriteUInt32(0); // unk
            pw.WriteUInt32(player.StatsManager.MissionWins);
            pw.WriteUInt32(player.StatsManager.MissionLosses);
            pw.WriteUInt32(player.StatsManager.MissionDrews);
            pw.WriteUInt16((ushort)player.StatsManager.SlackerPoints);
            pw.WriteUInt32(0);
            pw.WriteUInt32(1);

            var leaderboardPos = Game.Instance.Leaderboard.GetPosition(player.PlayerId);
            pw.WriteUInt32(leaderboardPos.Rank);
            pw.WriteInt32(leaderboardPos.Difference);

            var achievements = player.AchievementContainer.List;
            pw.WriteUInt8((byte)achievements.Count);

            foreach(var achievement in achievements)
            {
                pw.WriteUInt32(achievement);
                pw.WriteInt32(1);
            }

            pw.WriteBytes(new byte[1000]);

            return pw;
        }
        public PacketWriter PlayerInfoInspectorFailed()
        {
            PacketWriter pw = new PacketWriter((Opcode)693);
            return pw;
        }
        public PacketWriter ReceiveWhisper(string sender, string message)
        {
            PacketWriter pw = new PacketWriter((Opcode)738);
            pw.WriteUInt32(123123); // lol?
            ushort len = (ushort)(message.Length % 254);
            pw.WriteUInt16(len);

            pw.WriteWString(sender, 16);

            pw.WriteWString(message, len);
            return pw;
        }
        public PacketWriter ResetKillDeath(Player player, InventoryCard card)
        {
            PacketWriter pw = new PacketWriter((Opcode)845);
            pw.WriteUInt32(0);

            // InventoryCard
            pw.WriteUInt64(card.Id);        // 0 
            pw.WriteUInt32(card.ItemId);    // 8
            pw.WriteUInt8(10);              // 12
            pw.WriteUInt8(card.Type);       // 13
            pw.WriteUInt8(0);               // 14
            pw.WriteUInt8(card.IsGiftable ? (byte)1 : (byte)0); // 15
            pw.WriteBytes(new byte[6]);     // 16
            pw.WriteUInt32(card.TimeCreated);   // 22
            pw.WriteUInt8(card.IsOpened ? (byte)1 : (byte)0); // 26
            pw.WriteUInt16(card.IsActive ? (ushort)0 : (ushort)1); // 27
            pw.WriteUInt8(0);         // 28 hidden
            pw.WriteUInt8(0);         // 29
            pw.WriteUInt16(card.Period);    //
            pw.WriteUInt8(card.PeriodeType);
            pw.WriteUInt8(0);
            pw.WriteUInt16(card.BoostLevel);
            pw.WriteUInt8(card.BoostLevel > 0 ? (byte)1 : (byte)0);
            pw.WriteUInt8(0);
            pw.WriteBytes(new byte[4]);
            //
            pw.WriteUInt32(player.Experience);
            pw.WriteUInt32(player.Don);
            pw.WriteUInt32(0);
            pw.WriteUInt32(0);
            pw.WriteUInt32(0);
            pw.WriteUInt32(player.StatsManager.NormalWins);
            pw.WriteUInt32(player.StatsManager.NormalLosses);
            pw.WriteUInt32(player.StatsManager.NormalDrews);
            pw.WriteUInt32(0);
            pw.WriteUInt32(player.StatsManager.MissionWins);
            pw.WriteUInt32(player.StatsManager.MissionLosses);
            pw.WriteUInt32(player.StatsManager.MissionDrews);
            pw.WriteBytes(new byte[72]);
            pw.WriteUInt32(player.StatsManager.SlackerPoints);
            return pw;
        }
        public PacketWriter ResetWinLoss(Player player, InventoryCard card)
        {
            PacketWriter pw = new PacketWriter((Opcode)842);
            pw.WriteUInt32(0);
            // InventoryCard
            pw.WriteUInt64(card.Id);        // 0 
            pw.WriteUInt32(card.ItemId);    // 8
            pw.WriteUInt8(10);              // 12
            pw.WriteUInt8(card.Type);       // 13
            pw.WriteUInt8(0);               // 14
            pw.WriteUInt8(card.IsGiftable ? (byte)1 : (byte)0); // 15
            pw.WriteBytes(new byte[6]);     // 16
            pw.WriteUInt32(card.TimeCreated);   // 22
            pw.WriteUInt8(card.IsOpened ? (byte)1 : (byte)0); // 26
            pw.WriteUInt16(card.IsActive ? (ushort)0 : (ushort)1); // 27
            pw.WriteUInt8(0);         // 28 hidden
            pw.WriteUInt8(0);         // 29
            pw.WriteUInt16(card.Period);    //
            pw.WriteUInt8(card.PeriodeType);
            pw.WriteUInt8(0);
            pw.WriteUInt16(card.BoostLevel);
            pw.WriteUInt8(card.BoostLevel > 0 ? (byte)1 : (byte)0);
            pw.WriteUInt8(0);
            pw.WriteBytes(new byte[4]);
            //
            pw.WriteUInt32(player.Experience);
            pw.WriteUInt32(player.Don);
            pw.WriteUInt32(0);
            pw.WriteUInt32(player.StatsManager.Kills);
            pw.WriteUInt32(player.StatsManager.Deaths);
            pw.WriteUInt32(0);
            pw.WriteUInt32(0);
            pw.WriteUInt32(0);
            pw.WriteUInt32(0);
            pw.WriteUInt32(0);
            pw.WriteUInt32(0);
            pw.WriteUInt32(0);
            pw.WriteBytes(new byte[72]);
            pw.WriteUInt32(player.StatsManager.SlackerPoints);
            return pw;
        }
        public PacketWriter SendWhisper(string sender, string message)
        {
            PacketWriter pw = new PacketWriter((Opcode)739);
            pw.WriteUInt32(0);
            ushort len = (ushort)(sender.Length % 254);
            pw.WriteUInt16(len);
            pw.WriteWString(sender, 16);
            pw.WriteWString(message, len);
            return pw;
        }
        public PacketWriter UpdateCashBalance(uint cash)
        {
            PacketWriter pw = new PacketWriter((Opcode)832);
            pw.WriteUInt32(cash);
            return pw;
        }
        public PacketWriter UpdateCharacter(ushort character)
        {
            PacketWriter pw = new PacketWriter((Opcode)680);
            pw.WriteUInt16(character);
            return pw;
        }
        public PacketWriter UpdatePlayerRanking(LeaderboardPosition position)
        {
            PacketWriter pw = new PacketWriter((Opcode)792);
            pw.WriteBytes(new byte[4]);
            pw.WriteUInt32(position.Rank);
            pw.WriteUInt32(position.Difference);
            return pw;
        }
        #endregion

        #region Shop
        public PacketWriter CardPurchaseComplete(ShopItem item, List<InventoryCard> cards, uint newBalance)
        {
            PacketWriter pw = new PacketWriter((Opcode)804);
            pw.WriteUInt8(item.IsCash ? (byte)1 : (byte)0);
            pw.WriteUInt32(newBalance);
            pw.WriteUInt32(0);
            pw.WriteUInt8((byte)cards.Count);
            foreach(var card in cards)
            {
                // InventoryCard
                pw.WriteUInt64(card.Id);        // 0 
                pw.WriteUInt32(card.ItemId);    // 8
                pw.WriteUInt8(10);              // 12
                pw.WriteUInt8(card.Type);       // 13
                pw.WriteUInt8(0);               // 14
                pw.WriteUInt8(card.IsGiftable ? (byte)1 : (byte)0); // 15
                pw.WriteBytes(new byte[6]);     // 16
                pw.WriteUInt32(card.TimeCreated);   // 22
                pw.WriteUInt8(card.IsOpened ? (byte)1 : (byte)0); // 26
                pw.WriteUInt16(card.IsActive ? (ushort)0 : (ushort)1); // 27
                pw.WriteUInt8(0);         // 28 hidden
                pw.WriteUInt8(0);         // 29
                pw.WriteUInt16(card.Period);    //
                pw.WriteUInt8(card.PeriodeType);
                pw.WriteUInt8(0);
                pw.WriteUInt16(card.BoostLevel);
                pw.WriteUInt8(card.BoostLevel > 0 ? (byte)1 : (byte)0);
                pw.WriteUInt8(0);
                pw.WriteBytes(new byte[4]);
            }
            return pw;
        }
        public PacketWriter ShopItems(List<ShopItem> items)
        {
            PacketWriter pw = new PacketWriter((Opcode)798);
            ushort len = (ushort)items.Count;
            pw.WriteUInt16(len);
            pw.WriteUInt16(len);
            pw.WriteUInt16(len);
            foreach(var item in items)
            {
                pw.WriteUInt32(item.SeqId);
                pw.WriteUInt32(item.ItemId);
                pw.WriteUInt8(item.IsCash ? (byte)1 : (byte)0);
                pw.WriteUInt32(item.Price);
                pw.WriteUInt32(item.Stock >= 9999 || item.SoldCount < item.Stock ? (byte)1 : (byte)0);
                pw.WriteUInt8(item.ShopCategory);
            }
            return pw;
        }
        #endregion

        // testing
        public PacketWriter Send_884()
        {
            PacketWriter pw = new PacketWriter((Opcode)884);
            pw.WriteUInt32(0); // unk1
            pw.WriteUInt32(0); // unk2
            return pw;
        }

        public PacketWriter Send_729() // Lobby Send Memo Rsp
        {
            PacketWriter pw = new PacketWriter((Opcode)729);
            // epty
            return pw;
        }

        public PacketWriter Send_731() // Lobby Recv Memo
        {
            PacketWriter pw = new PacketWriter((Opcode)731);
            // empty
            return pw;
        }
        
        public PacketWriter Send_699() // LobbyBuddyAddFail
        {
            PacketWriter pw = new PacketWriter((Opcode)699);
            // empty
            return pw;
        }

        public PacketWriter Send_879() // nothing
        {
            PacketWriter pw = new PacketWriter((Opcode)879);
            // todo
            return pw;
        }

        public PacketWriter Send_887() // nothing
        {
            PacketWriter pw = new PacketWriter((Opcode)887);
            // todo
            return pw;
        }

        public PacketWriter TradeFailed() // trade failed
        {
            PacketWriter pw = new PacketWriter((Opcode)877);
            // todo
            return pw;
        }

        public PacketWriter TradeCanceledBYOther() // trade cancle? TRADE_REQUEST_CANCEL
        {
            PacketWriter pw = new PacketWriter((Opcode)881);
            // todo
            return pw;
        }

        public PacketWriter TradeResp()
        {
            PacketWriter pw = new PacketWriter((Opcode)880);
            // todo
            return pw;
        }

        public PacketWriter TradeRespFailBYOther() // OnLsTradeRespFail
        {
            PacketWriter pw = new PacketWriter((Opcode)881);
            // todo
            return pw;
        }

        public PacketWriter Send_882() // trade rejected
        {
            PacketWriter pw = new PacketWriter((Opcode)882);
            // todo
            return pw;
        }

        

        public PacketWriter TradeAccepted() // TradeAccepted
        {
            PacketWriter pw = new PacketWriter((Opcode)883);
            // todo
            return pw;
        }

        public PacketWriter Send_889() // item response
        {
            PacketWriter pw = new PacketWriter((Opcode)889);
            // todo
            pw.WriteUInt32(0xff); // 8
            pw.WriteUInt8(0);
            return pw;
        }

        public PacketWriter Send_891() // unk
        {
            PacketWriter pw = new PacketWriter((Opcode)891);
            pw.WriteUInt8(0); // targetId?

            // _DWORD interpreted as _BYTE
            pw.WriteUInt8(0); // unk2
            pw.Write(new byte[3]); // gap?

            pw.WriteUInt32(0); // unk3 qmemcpy src
            return pw;
        }
        public PacketWriter Send_892() // unk
        {
            PacketWriter pw = new PacketWriter((Opcode)892);
            // todo
            return pw;
        }

        public PacketWriter Send_895() // unk
        {
            PacketWriter pw = new PacketWriter((Opcode)895);
            // todo
            return pw;
        }

        public PacketWriter Send_896() // unk
        {
            PacketWriter pw = new PacketWriter((Opcode)896);
            // todo
            return pw;
        }

        
        public PacketWriter Send_885()
        {
            PacketWriter pw = new PacketWriter((Opcode)885);
            // TODO?
            return pw;
        }


        public PacketWriter SendTradeCanceled() // unk
        {
            PacketWriter pw = new PacketWriter((Opcode)885);
            // todo
            return pw;
        }
    }
}
