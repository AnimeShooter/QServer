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

        }
    }
}
