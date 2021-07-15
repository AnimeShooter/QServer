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

            // TODO
            //manager.Player.EquipmentManager. SetArmor
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

            // TODO
            //manager.Player.EquipmentManager.SetWeapons
        }

        public static void HandleRequestEquippedSkillCards(PacketReader packet, ConnServer manager)
        {
            //Thread.Sleep(-1);
            // TODO
            var skills = new InventoryCard[] { 
                new InventoryCard()
                {
                    Id = 1,
                },
                new InventoryCard()
                {
                    Id = 2,
                    
                },
                new InventoryCard()
                {
                    Id = 3
                }
            }; // manager.Player.EquipmentManager.SkillCards;
            manager.Send(LobbyManager.Instance.EquippedSkillCards(skills));
        }
        #endregion

        #region Friend
        public static void HandleAcceptIncomingFriendRequestEvent(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }
        public static void HandleCancleOutgoingFriendRequestEvent(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }
        public static void HandleDenyIncomingFriendRequestEvent(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }
        public static void HandleRemoveFriendEvent(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }
        public static void HandleRequestFriendList(PacketReader packet, ConnServer manager)
        {
            List<Friend> friends = manager.Player.FriendManager.List();
            manager.Send(LobbyManager.Instance.FriendList(friends));
        }
        public static void HandleSendFriendRequestEvent(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Gameroom
        public static void HandleRequestGameRoomsEvent(PacketReader packet, ConnServer manager)
        {
            //throw new NotImplementedException();
        }
        public static void HandleRequestGameSettingsEvent(PacketReader packet, ConnServer manager)
        {
            //throw new NotImplementedException();
        }
        #endregion

        #region Inventory
        public static void HandleDeleteCard(PacketReader packet, ConnServer manager)
        {
            var cardId = packet.ReadUInt64();
            var player = manager.Player;
            if (player == null)
                return;

            player.InventoryManager.DeleteCard(cardId);
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
            throw new NotImplementedException();
        }
        public static void HandleOpenGift(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }
        public static void HandleRequestGifts(PacketReader packet, ConnServer manager)
        {
            //throw new NotImplementedException();
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
        #endregion

        #region Misc
        public static void HandleUseCraneEvent(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        public static void HandleRequestPlayerRanking(PacketReader packet, ConnServer manager)
        {
            var playerId = packet.ReadUInt32();
            var targetPlayer = Game.Instance.GetPlayer(playerId);
            if (targetPlayer == null)
                return;

            manager.Send(LobbyManager.Instance.PlayerInfoInspector(targetPlayer));
        }
        public static void HandleRestKillDeathEvent(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }
        public static void HandleResetWinLossEvent(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }
        public static void HandleWhisperEvent(PacketReader packet, ConnServer manager)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Shop
        public static void HandleBuyCardEvent(PacketReader packet, ConnServer manager)
        {
            //uint seqId = packet.ReadUInt32();
            //if (manager.Player != null)
            //    Game.Instance.ShopManager.Buy(manager.Player, seqId);
        }
        public static void HandleRequestShopItems(PacketReader packet, ConnServer manager)
        {
            List<ShopItem> items = Game.Instance.ShopManager.List();
            manager.Send(LobbyManager.Instance.ShopItems(items));
        }
        public static void HandleRequestShopPackages(PacketReader packet, ConnServer manager)
        {
            // TODO
            //throw new NotImplementedException();
        }
        #endregion

        public static void HandleLobbyLogin(PacketReader packet, ConnServer manager)
        {
            byte[] uuid = packet.ReadBytes(16);
            uint userId = 0xFFFF0102;
            // databse UUID to ID

            bool isBanned = false;
            if (isBanned)
            {
                manager.Send(LobbyManager.Instance.Banned());
                manager.CloseSocket();
                return;
            }

            var player = Game.Instance.CreatePlayer(manager, userId);

            manager.Send(LobbyManager.Instance.Authenticated(player));
        }
    }
}
