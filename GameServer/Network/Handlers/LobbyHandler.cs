using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Util;
using Qserver.GameServer.Qpang;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Managers;
using Qserver.GameServer.Network.Packets;
using System.Threading;

namespace Qserver.GameServer.Network.Handlers
{
    public class LobbyHandler
    {
        #region Channel
        public static void HandleChannelList(PacketReader packet, ConnServer manager)
        {
            var list = Game.Instance.ChannelManager.List();
            manager.Send(LobbyManager.Instance.ChannelList(list));
        }
        public static void HandleChannelHost(PacketReader packet, ConnServer manager)
        {
            uint channelId = packet.ReadUInt32();
            Channel channel = Game.Instance.ChannelManager.GetChannel(channelId);
            if(manager.Player.LoginTime > DateTime.UtcNow.AddSeconds(5) && manager.Player.TestRealm != (channel.TestMode == 1)) // 255 is default non set
            {
                manager.Player.Close(); // Not allowed to switch server
                return;
            }
            manager.Player.TestRealm = channel.TestMode == 1;  // set mode to prevent swapping lateron

            // NOTE: crashes
            //if(manager.Player.TestRealm)
            //    manager.Send(LobbyManager.Instance.ShopItems(new List<ShopItem>())); // erase the shop items

            manager.Send(LobbyManager.Instance.ChannelHost(channel));
        }
        #endregion Channel

        #region Equipment
        public static void HandleEquipArmor(PacketReader packet, ConnServer manager)
        {
            ushort characterIndex = packet.ReadUInt16();
            ulong[] armor = new ulong[9];

            for(int i = 0; i < armor.Length; i++)
                armor[i] = packet.ReadUInt64();

            manager.Player.EquipmentManager.SetArmor(characterIndex, armor);
        }
        public static void HandleEquipWeapon(PacketReader packet, ConnServer manager)
        {
            ushort characterIndex = packet.ReadUInt16();
            ulong[] weapons = new ulong[4];

            for(int i = 0; i < weapons.Length; i++)
            {
                weapons[i] = packet.ReadUInt64();
                packet.ReadUInt64(); // unk
            }

            manager.Player.EquipmentManager.SetWeapons(characterIndex, weapons);
        }

        public static void HandleRequestEquippedSkillCards(PacketReader packet, ConnServer manager)
        {
            var skills = manager.Player.EquipmentManager.GetSkillCards();
            manager.Send(LobbyManager.Instance.EquippedSkillCards(skills));
        }
        #endregion

        #region Friend
        public static void HandleAcceptIncomingFriendRequestEvent(PacketReader packet, ConnServer manager)
        {
            var playerId = packet.ReadUInt32();
            var player = manager.Player;
            var target = Game.Instance.GetPlayer(playerId);
            if(target != null)
            {
                player.FriendManager.AcceptIncoming(target);
                target.FriendManager.OnOutgoingAccepted(player);
            }
        }
        public static void HandleCancleOutgoingFriendRequestEvent(PacketReader packet, ConnServer manager)
        {
            var playerId = packet.ReadUInt32();
            var player = manager.Player;
            var target = Game.Instance.GetPlayer(playerId);
            if(target != null)
            {
                player.FriendManager.RemoveOutgoing(playerId);
                target.FriendManager.RemoveIncoming(player.PlayerId);

                target.SendLobby(LobbyManager.Instance.IncomingFriendCancelled(player));
            }

            manager.Send(LobbyManager.Instance.CancelOutgoingFriend(playerId));
        }
        public static void HandleDenyIncomingFriendRequestEvent(PacketReader packet, ConnServer manager)
        {
            var playerId = packet.ReadUInt32();
            var player = manager.Player;
            var target = Game.Instance.GetPlayer(playerId);
            if(target != null)
            {
                player.FriendManager.RemoveIncoming(playerId);
                target.FriendManager.RemoveOutgoing(player.PlayerId);

                target.SendLobby(LobbyManager.Instance.OutgoingFriendCancelled(player));
            }
            manager.Send(LobbyManager.Instance.DenyIncomingFriend(playerId));
        }
        public static void HandleRemoveFriendEvent(PacketReader packet, ConnServer manager)
        {
            var friendId = packet.ReadUInt32();
            var player = manager.Player;
            var friend = Game.Instance.GetPlayer(friendId);
            if(friend != null)
            {
                player.FriendManager.Remove(friend.PlayerId);
                friend.FriendManager.OnRemove(player.PlayerId);
            }
        }
        public static void HandleRequestFriendList(PacketReader packet, ConnServer manager)
        {
            List<Friend> friends = manager.Player.FriendManager.List();
            manager.Send(LobbyManager.Instance.FriendList(friends));
        }
        public static void HandleSendFriendRequestEvent(PacketReader packet, ConnServer manager)
        {
            packet.ReadBytes(12);
            string name = packet.ReadWString(16);

            if (name == null || name.Length < 4 || name.Length > 16)
            {
                manager.Send(LobbyManager.Instance.Send_699());
                return;
            }

            var player = manager.Player;
            if (name == player.Name)
            {
                manager.Send(LobbyManager.Instance.Send_699());
                return;
            }

            if (!player.FriendManager.HasOutgoingSlot())
            {
                manager.Send(LobbyManager.Instance.Send_699());
                return;
            }

            var target = Game.Instance.GetPlayer(name);

            if (target == null)
            {
                manager.Send(LobbyManager.Instance.Send_699());
                return;
            }

            if (!target.FriendManager.HasIncomingSlot())
            {
                manager.Send(LobbyManager.Instance.Send_699());
                return;
            }

            if (player.FriendManager.Contains(target.PlayerId) || target.FriendManager.Contains(player.PlayerId))
            {
                manager.Send(LobbyManager.Instance.Send_699());
                return;
            }

            player.FriendManager.AddOutgoingFriend(target);
            target.FriendManager.AddIncomingFriend(player);
        }
        #endregion

        #region Gameroom
        public static void HandleRequestGameRoomsEvent(PacketReader packet, ConnServer manager)
        {
            var rooms = Game.Instance.RoomManager.List();
            manager.Send(LobbyManager.Instance.RoomList(rooms));
        }
        public static void HandleRequestGameSettingsEvent(PacketReader packet, ConnServer manager)
        {
            var rooms = Game.Instance.RoomManager.List();
            manager.Send(LobbyManager.Instance.RoomList(rooms));
            manager.Send(LobbyManager.Instance.UpdateGameSettings(Settings.SERVER_IP, (ushort)Settings.SERVER_PORT_ROOM, true));
        }
        #endregion

        #region Inventory
        public static void HandleLobbyTrade(PacketReader packet, ConnServer manager)
        {
            uint playerId = packet.ReadUInt32(); // target (875)
            var target = Game.Instance.GetOnlinePlayer(playerId);
            if (target == null)
                return;

            target.SendLobby(LobbyManager.Instance.SendTradeRequest(playerId));
            manager.Send(LobbyManager.Instance.TradeResponse(playerId));
        }

        public static void HandleLobbyTradeAct(PacketReader packet, ConnServer manager)
        {
            // no opcode?
        }

        public static void HandleDeleteCard(PacketReader packet, ConnServer manager)
        {
            var cardId = packet.ReadUInt64();
            var player = manager.Player;
            if (player == null)
                return;

            player.InventoryManager.DeleteCard(cardId); //
        }
        public static void HandleDisableFunctionCardEvent(PacketReader packet, ConnServer manager)
        {
            var cardId = packet.ReadUInt64();
            var player = manager.Player;
            if (player == null)
                return;

            var card = player.InventoryManager.Get(cardId);

            if (ItemID.IsEquippableFunction(card.ItemId))
                player.InventoryManager.SetCardActive(card.Id, false);
        }
        public static void HandleEnableFunctionCardEvent(PacketReader packet, ConnServer manager)
        {
            var cardId = packet.ReadUInt64();
            var player = manager.Player;
            if (player == null)
                return;

            var card = player.InventoryManager.Get(cardId);

            if (ItemID.IsEquippableFunction(card.ItemId))
                player.InventoryManager.SetCardActive(card.Id, true);
        }
        public static void HandleExtendCardEvent(PacketReader packet, ConnServer manager)
        {
            ulong cardId = packet.ReadUInt64();
            uint itemId = packet.ReadUInt32();
            byte unk1 = packet.ReadUInt8();
            uint price = packet.ReadUInt32();
            uint seqId = packet.ReadUInt32();
            byte flag = packet.ReadUInt8();
            ushort kek = packet.ReadUInt16(); // lol xD

            // NOTE: Unused?
        }
        public static void HandleGiftCardEvent(PacketReader packet, ConnServer manager)
        {
            var player = manager.Player;
            if (player == null)
                return;

            var nickname = packet.ReadWString(16); // read str terminated
            var cardId = packet.ReadUInt64();
            var idk = packet.ReadUInt32();

            var card = player.InventoryManager.Get(cardId);

            if (!card.IsGiftable)
                return;

            var isEquipped = player.EquipmentManager.HasEquipped(cardId);
            if (isEquipped)
                return;

            var targetPlayer = Game.Instance.GetPlayer(nickname);
            if (targetPlayer == null)
                return;

            if (!targetPlayer.InventoryManager.HasSpace() || !targetPlayer.InventoryManager.HasGiftSpace())
                return;

            player.InventoryManager.GiftCard(card, targetPlayer);
        }
        public static void HandleOpenGift(PacketReader packet, ConnServer manager)
        {
            var cardId = packet.ReadUInt64();
            manager.Player.InventoryManager.OpenGift(cardId);
        }
        public static void HandleRequestGifts(PacketReader packet, ConnServer manager)
        {
            var player = manager.Player;
            if (player == null)
                return;

            var gifts = player.InventoryManager.ListGifts();
            manager.Send(LobbyManager.Instance.Gifts(gifts));
        }
        public static void HandleRequestInventory(PacketReader packet, ConnServer manager)
        {
            List<InventoryCard> cards = manager.Player.InventoryManager.List();
            manager.Send(LobbyManager.Instance.Inventory(cards));
        }
        #endregion

        #region Memo
        public static void HandleRequestMemos(PacketReader packet, ConnServer manager)
        {
            if (manager.Player == null)
                return;

            List<Memo> memos = manager.Player.MemoManager.List();
            manager.Send(LobbyManager.Instance.Memos(memos));
        }

        public static void HandleMemoDelete(PacketReader packet, ConnServer manager)
        {
            // TODO
        }

        public static void HandleSendMemo(PacketReader packet, ConnServer manager)
        {
            // TODO
        }
        public static void HandleOpenMemo(PacketReader packet, ConnServer manager)
        {
            // TODO
        }
        #endregion

        #region Misc
        public static void HandleUseCraneEvent(PacketReader packet, ConnServer manager)
        {
            var times = packet.ReadUInt16();
            var player = manager.Player;

            if (player.TestRealm)
                return;

            if (player.InventoryManager.List().Count >= 200 - times)
                return;

            var craneManager = Game.Instance.CraneManger;
            if (!craneManager.Enabled)
                return;

            uint coinsRequired = 0;

            switch(times)
            {
                case 1:
                    coinsRequired = 200;
                    break;
                case 3:
                    coinsRequired = 500;
                    break;
                case 7: coinsRequired = 1200;
                    break;
                default:
                    return;
            }

            if (coinsRequired > player.Coins)
                return;

            player.RemoveCoins(coinsRequired);

            List<InventoryCard> cards = new List<InventoryCard>();

            for(int i = 0; i < times; i++)
            {
                var invCard = craneManager.GetRandomItem();
                player.InventoryManager.StoreCard(invCard);
                cards.Add(invCard);
            }

            player.SendLobby(LobbyManager.Instance.UseCrainSuccess(player, cards)); // TODO: fix active bug? (temp fix: relog)

        }
        public static void HandleRedeemCode(PacketReader packet, ConnServer manager)
        {
            byte[] key = packet.ReadBytes(14);
            packet.ReadBytes(15);  // unk

            //  handle code
            bool status = Game.Instance.CouponManager.ConsumeCoupon(manager.Player, Encoding.ASCII.GetString(key));

            if (!status)
                manager.Send(LobbyManager.Instance.Send_853());
            else
                manager.Send(LobbyManager.Instance.Send_852(manager.Player.Don, manager.Player.Cash));
        } 
        #endregion

        #region Player
        public static void HandleChangeCharacterEvent(PacketReader packet, ConnServer manager)
        {
            ushort characterId = packet.ReadUInt16();
            if (manager.Player == null)
                return;

            var characters = manager.Player.EquipmentManager.UnlockerCharacters;
            if (characters.Contains(characterId))
                manager.Player.Character = characterId;
        }
        public static void HandleRequestCashBalance(PacketReader packet, ConnServer manager)
        {
            uint cash = manager.Player.Cash;
            manager.Send(LobbyManager.Instance.UpdateCashBalance(cash));
        }
        public static void HandleRequestPlayerInfo(PacketReader packet, ConnServer manager)
        {
            var playerId = packet.ReadUInt32();
            var player = Game.Instance.GetPlayer(playerId);
            if (player == null)
                return;

            manager.Send(LobbyManager.Instance.PlayerInfoInspector(player));
        }
        public static void HandleRequestPlayerRanking(PacketReader packet, ConnServer manager)
        {
            //var playerId = packet.ReadUInt32();
            //var targetPlayer = Game.Instance.GetPlayer(playerId);
            //if (targetPlayer == null)
                return;

            //manager.Send(LobbyManager.Instance.PlayerInfoInspector(targetPlayer));
        }
        public static void HandleResetKillDeathEvent(PacketReader packet, ConnServer manager)
        {
            ulong cardId = packet.ReadUInt64();
            var player = manager.Player;
            if (player == null)
                return;

            var card = player.InventoryManager.Get(cardId);
            if (card.ItemId != (uint)Items.KD_CLEANER || card.Period == 0)
                return;

            player.StatsManager.ClearKD();
            if (card.PeriodeType != 254)
                player.InventoryManager.DeleteCard(cardId);

            manager.Send(LobbyManager.Instance.ResetKillDeath(player, card));
        }
        public static void HandleResetWinLossEvent(PacketReader packet, ConnServer manager)
        {
            ulong cardId = packet.ReadUInt64();
            var player = manager.Player;
            if (player == null)
                return;

            var card = player.InventoryManager.Get(cardId);
            if (card.ItemId != (uint)Items.WL_CLEANER || card.Period == 0)
                return;

            player.StatsManager.ClearWL();
            if (card.PeriodeType != 254)
                player.InventoryManager.DeleteCard(cardId);

            manager.Send(LobbyManager.Instance.ResetKillDeath(player, card));
        }
        public static void HandleWhisperEvent(PacketReader packet, ConnServer manager)
        {
            packet.ReadBytes(4);

            var len = packet.ReadUInt16() % 254;
            string nickname = packet.ReadWString(16);
            string messagge = packet.ReadWString(len-1); // not always?

            var player = Game.Instance.GetOnlinePlayer(nickname);
            if (player == null)
                return;

            player.Whisper(manager.Player.Name, messagge);
            manager.Send(LobbyManager.Instance.SendWhisper(nickname, messagge));
        }
        #endregion

        #region Shop
        public static void HandleBuyCardEvent(PacketReader packet, ConnServer manager)
        {
            uint seqId = packet.ReadUInt32();
            if (manager.Player != null)
                Game.Instance.ShopManager.Buy(manager.Player, seqId);
        }
        public static void HandleRequestShopItems(PacketReader packet, ConnServer manager)
        {
            List<ShopItem> items = manager.Player.TestRealm ? new List<ShopItem>() : Game.Instance.ShopManager.List();
            manager.Send(LobbyManager.Instance.ShopItems(items));
        }
        public static void HandleRequestShopPackages(PacketReader packet, ConnServer manager)
        {
            // TODO
            //throw new NotImplementedException();
        }
        #endregion

        #region AntiCheating
        public static void HandleAntiCheat(PacketReader packet, ConnServer manager)
        {
            if (manager.Player == null)
                return;

            manager.Player.AntiCheat = Util.Util.Timestamp();
            Log.Message(LogType.NORMAL, $"Anti-Cheat hearthbeat for {manager.Player.Name}!");
        }
        #endregion

        // testing
        public static void Handle_728(PacketReader packet, ConnServer manager) // Lobby Send Memo
        {
            uint playerId = packet.ReadUInt32();
            string nickname = packet.ReadString(); // target?
            string message = packet.ReadString();

            // ok?
            manager.Send(LobbyManager.Instance.Send_729());
        }



        public static void HandleLobbyLogin(PacketReader packet, ConnServer manager)
        {
            byte[] uuid = packet.ReadBytes(16);
            uint userId = Game.Instance.UsersRepository.GetUserId(Encoding.ASCII.GetString(uuid)).Result;

            if (userId == 0)
            {
                manager.Send(LobbyManager.Instance.VerificationFailure());
                manager.CloseSocket();
                return;
            }

            bool isBanned = false; // Game.Instance.BanManager.
            if (isBanned)
            {
                manager.Send(LobbyManager.Instance.Banned());
                manager.CloseSocket();
                return;
            }

            uint playerId = Game.Instance.PlayersRepository.GetPlayerId(userId).Result;
            var player = Game.Instance.CreatePlayer(manager, playerId);

            manager.Send(LobbyManager.Instance.Authenticated(player));
        }
    }
}
