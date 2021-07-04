using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Qserver.Util;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Packets;

namespace Qserver.GameServer.Packets
{
    public static class PacketManager
    {
        public static Dictionary<Opcode, HandlePacket> OpcodeHandlers = new Dictionary<Opcode, HandlePacket>();
        public delegate void HandlePacket(PacketReader packet, ConnServer server);

        public static void DefineOpcodeHandler(Opcode opcode, HandlePacket handler)
        {
            OpcodeHandlers[opcode] = handler;
        }

        public static bool InvokeHandler(PacketReader reader, ConnServer manager, Opcode opcode)
        {
            if (OpcodeHandlers.ContainsKey(opcode))
            {
                OpcodeHandlers[opcode].Invoke(reader, manager);
                return true;
            }
            else
                Log.Message(LogType.ERROR, $"Unknown OpCode: {opcode}");
            return false;
        }

    }
}
