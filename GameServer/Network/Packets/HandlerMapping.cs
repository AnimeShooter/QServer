using Qserver.GameServer.Network.Handlers;
using Qserver.GameServer.Network;

namespace Qserver.GameServer.Packets
{
    public class HandlerMapping
    {
        public static void InitPacketHandlers()
        {
            // Authentication
            PacketManager.DefineOpcodeHandler(Opcode.KEY_EXCHANGE, AuthHandler.HandleHandshake);
            PacketManager.DefineOpcodeHandler(Opcode.AUTH_LOGIN, AuthHandler.HandleLoginRequest);

            // Lobby
            PacketManager.DefineOpcodeHandler(Opcode.LOBBY_LOGIN, ParkHandler.HandleLobbyLogin);
            //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_EQUIP_ARMOUR, ParkHandler.);
            //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_EQUIP_WEAPON, ParkHandler.);
            //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_EQUIPPED_SKILLS, ParkHandler.);
            //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_DROP_CARD, ParkHandler.);
            //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_SWAP_CHARACTER, ParkHandler.);
            //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_PLAYERINFO, ParkHandler.);
            //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_BUDDIES, ParkHandler.);
            //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_FRIEND_INVITE, ParkHandler.);
            //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_ACCEPT_INCOMING_FRIEND, ParkHandler.);
            //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_DENY_INCOMING_FRIEND, ParkHandler.);
            //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_OUTGOING_FRIEND_DENIED, ParkHandler.);
            //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_REMOVE_FRIEND, ParkHandler.);
            //PacketManager.DefineOpcodeHandler(Opcode.LOBBY_MEMOS, ParkHandler.);
            //add(738, new WhisperEvent());
            //add(742, new OpenGift());
            //add(745, new RequestGifts());
            //add(758, new RequestGameRoomsEvent());
            PacketManager.DefineOpcodeHandler(Opcode.LOBBY_CHANNELS, ParkHandler.HandleChannelList);
            PacketManager.DefineOpcodeHandler(Opcode.LOBBY_CHANNEL_CONNECT, ParkHandler.HandleChannelHost); //add(766, new RequestChannelHost());
            //add(769, new RequestGameSettingsEvent());
            //add(780, new RequestInventory());
            //add(791, new RequestPlayerRanking());
            //add(797, new RequestShopItems());
            //add(800, new RequestShopPackages());
            //add(803, new BuyCardEvent());
            //add(809, new ExtendCardEvent());
            //add(812, new GiftCardEvent());
            //add(831, new RequestCashBalance());
            //add(834, new EnableFunctionCardEvent());
            //add(841, new ResetWinLossEvent());
            //add(844, new ResetKillDeathEvent());
            //add(861, new DisableFunctionCardEvent());
            //add(897, new UseCraneEvent());


        }
    }
}
