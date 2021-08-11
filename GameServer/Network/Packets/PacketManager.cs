using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Qserver.Util;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Packets;
using System.Diagnostics;
using System.Threading;

namespace Qserver.GameServer.Packets
{
    public static class PacketManager
    {
        public static Dictionary<Opcode, HandlePacket> OpcodeHandlers = new Dictionary<Opcode, HandlePacket>();
        public static Dictionary<Opcode, HandlePacket> UnsafeOpcodeHandlers = new Dictionary<Opcode, HandlePacket>();
        public delegate void HandlePacket(PacketReader packet, ConnServer server);

        public static void DefineOpcodeHandler(Opcode opcode, HandlePacket handler)
        {
            OpcodeHandlers[opcode] = handler;
        }

        public static void DefineUnsafeOpcodeHandler(Opcode opcode, HandlePacket handler)
        {
            UnsafeOpcodeHandlers[opcode] = handler;
        }

        public static bool InvokeHandler(PacketReader reader, ConnServer manager, Opcode opcode)
        {
            try
            {
                if (OpcodeHandlers.ContainsKey(opcode))
                {
                    OpcodeHandlers[opcode].Invoke(reader, manager);
                    return true;
                }
                else if(UnsafeOpcodeHandlers.ContainsKey(opcode))
                {
                    // TODO:
                }
                else
                    Log.Message(LogType.ERROR, $"Unknown OpCode: {opcode}");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        public static void CreateSafeThread()
        {
            new Thread(new ThreadStart(ThreadSafe)).Start();
            void ThreadSafe()
            {
                while(true)
                {

                    Thread.Sleep(1);
                }
            }
        }


    }
}
