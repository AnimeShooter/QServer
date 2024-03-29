﻿using Qserver.GameServer.Network.Handlers;
using Qserver.GameServer.Network;

namespace Qserver.GameServer.Packets
{
    public class HandlerMapping
    {
        public static void InitPacketHandlers(bool auth, bool lobby, bool square)
        {
            // Authentication
            if(auth)
                PacketManager.DefineOpcodeHandler(Opcode.AUTH_LOGIN, AuthHandler.HandleLoginRequest);

            // Always do key exchange?
            PacketManager.DefineOpcodeHandler(Opcode.KEY_EXCHANGE, AuthHandler.HandleHandshake); 

            // Lobby
            if (lobby)
            {
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_LOGIN, LobbyHandler.HandleLobbyLogin);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_EQUIP_ARMOUR, LobbyHandler.HandleEquipArmor);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_EQUIP_WEAPON, LobbyHandler.HandleEquipWeapon);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_EQUIPPED_SKILLS, LobbyHandler.HandleRequestEquippedSkillCards);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_EQUIP_SKILL, LobbyHandler.HandleEquipSkillCard);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_DROP_CARD, LobbyHandler.HandleDeleteCard);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_SWAP_CHARACTER, LobbyHandler.HandleChangeCharacterEvent);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_PLAYERINFO, LobbyHandler.HandleRequestPlayerInfo);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_BUDDIES, LobbyHandler.HandleRequestFriendList);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_FRIEND_INVITE, LobbyHandler.HandleSendFriendRequestEvent);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_ACCEPT_INCOMING_FRIEND, LobbyHandler.HandleAcceptIncomingFriendRequestEvent);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_DENY_INCOMING_FRIEND, LobbyHandler.HandleDenyIncomingFriendRequestEvent);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_REMOVE_OUTGOING_FRIEND, LobbyHandler.HandleCancleOutgoingFriendRequestEvent);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_REMOVE_FRIEND, LobbyHandler.HandleRemoveFriendEvent);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_MEMOS, LobbyHandler.HandleRequestMemos);
                //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_DELETE_MEMO, LobbyHandler.HandleMemoDelete);
                //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_SEND_MEMO, LobbyHandler.HandleSendMemo);
                //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_OPEN_MEMO, LobbyHandler.HandleOpenMemo);
                PacketManager.DefineOpcodeHandler((Opcode)738, LobbyHandler.HandleWhisperEvent);
                PacketManager.DefineOpcodeHandler((Opcode)742, LobbyHandler.HandleOpenGift);
                PacketManager.DefineOpcodeHandler((Opcode)745, LobbyHandler.HandleRequestGifts);
                PacketManager.DefineOpcodeHandler((Opcode)758, LobbyHandler.HandleRequestGameRoomsEvent);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_CHANNELS, LobbyHandler.HandleChannelList);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_CHANNEL_CONNECT, LobbyHandler.HandleChannelHost); //add(766, new RequestChannelHost());
                PacketManager.DefineOpcodeHandler((Opcode)769, LobbyHandler.HandleRequestGameSettingsEvent);
                PacketManager.DefineOpcodeHandler((Opcode)780, LobbyHandler.HandleRequestInventory);
                PacketManager.DefineOpcodeHandler((Opcode)791, LobbyHandler.HandleRequestPlayerRanking);
                PacketManager.DefineOpcodeHandler((Opcode)797, LobbyHandler.HandleRequestShopItems);
                PacketManager.DefineOpcodeHandler((Opcode)800, LobbyHandler.HandleRequestShopPackages);
                PacketManager.DefineOpcodeHandler((Opcode)803, LobbyHandler.HandleBuyCardEvent);
                PacketManager.DefineOpcodeHandler((Opcode)809, LobbyHandler.HandleExtendCardEvent);
                PacketManager.DefineOpcodeHandler((Opcode)812, LobbyHandler.HandleGiftCardEvent);
                PacketManager.DefineOpcodeHandler((Opcode)831, LobbyHandler.HandleRequestCashBalance);
                PacketManager.DefineOpcodeHandler((Opcode)834, LobbyHandler.HandleEnableFunctionCardEvent);
                PacketManager.DefineOpcodeHandler((Opcode)841, LobbyHandler.HandleResetWinLossEvent);
                PacketManager.DefineOpcodeHandler((Opcode)844, LobbyHandler.HandleResetKillDeathEvent);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_REDEEM_CODE, LobbyHandler.HandleRedeemCode);
                PacketManager.DefineOpcodeHandler((Opcode)861, LobbyHandler.HandleDisableFunctionCardEvent);
                PacketManager.DefineOpcodeHandler((Opcode)897, LobbyHandler.HandleUseCraneEvent);

                // anti-cheat
                PacketManager.DefineOpcodeHandler(Opcode.LEET_ANTI_CHEET, LobbyHandler.HandleAntiCheat);

                // test 
                PacketManager.DefineOpcodeHandler((Opcode)903, LobbyHandler.Handle_903); // boost request
                PacketManager.DefineOpcodeHandler((Opcode)906, LobbyHandler.Handle_906); // chest open

                // memo
                PacketManager.DefineOpcodeHandler((Opcode)728, LobbyHandler.Handle_728); // Lobby Send Memo

                // trade
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_TRADE, LobbyHandler.HandleTradeRequest);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_879, LobbyHandler.Handle_879);
                PacketManager.DefineOpcodeHandler(Opcode.LOBBY_TRADE_ACT, LobbyHandler.HandleTradeAct);
                PacketManager.DefineOpcodeHandler((Opcode)888, LobbyHandler.HandleTradeItem); // trade item
            }

            // Square
            if (square)
            {
                PacketManager.DefineOpcodeHandler(Opcode.SQUARE_LOGIN, SquareHandler.HandleConnectRequest); // connect request
                PacketManager.DefineOpcodeHandler(Opcode.SQUARE_PLAYER_JOIN, SquareHandler.HandleRequestPlayers);
                PacketManager.DefineOpcodeHandler((Opcode)6510, SquareHandler.HandleUpdatePosition);
                PacketManager.DefineOpcodeHandler((Opcode)6514, SquareHandler.HandleLeftInventory);
                PacketManager.DefineOpcodeHandler((Opcode)6526, SquareHandler.HandleChatRequest);
                PacketManager.DefineOpcodeHandler((Opcode)6530, SquareHandler.HandleReloadSquareEvent);
                PacketManager.DefineOpcodeHandler(Opcode.SQUARE_JOIN_PARK, SquareHandler.HandleSquareLogin); // aka JoinSquare
                PacketManager.DefineOpcodeHandler((Opcode)6544, SquareHandler.HandleUpdateStateEvent);
                PacketManager.DefineOpcodeHandler((Opcode)6557, SquareHandler.HandleEmoteEevent);
            }
        }
    }
}
