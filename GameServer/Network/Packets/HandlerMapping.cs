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
            PacketManager.DefineOpcodeHandler(Opcode.LOBBY_LOGIN, LobbyHandler.HandleLobbyLogin);
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
            PacketManager.DefineOpcodeHandler(Opcode.LOBBY_CHANNELS, LobbyHandler.HandleChannelList);
            PacketManager.DefineOpcodeHandler(Opcode.LOBBY_CHANNEL_CONNECT, LobbyHandler.HandleChannelHost); //add(766, new RequestChannelHost());
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

            // Square
            PacketManager.DefineOpcodeHandler(Opcode.SQUARE_LOGIN, SquareHandler.HandleConnectRequest); // connect request
            //add(6506, new RequestPlayers());
            //add(6510, new UpdatePosition());
            //add(6514, new LeftInventory());
            //add(6526, new ChatRequest());
            //add(6530, new ReloadSquareEvent());
            PacketManager.DefineOpcodeHandler(Opcode.SQUARE_JOIN_PARK, SquareHandler.HandleSquareLogin); // aka JoinSquare
            //add(6537, new JoinSquare());
            //add(6544, new UpdateStateEvent());
            //add(6557, new EmoteEvent());


        }
    }
}
