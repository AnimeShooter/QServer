using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNL.Entities;
using TNL.Utils;
using TNL.Data;
using TNL.Types;

namespace Qserver.GameServer.Qpang
{
    public class GCArrangedConn : GameNetEvent
    {
        private static NetClassRepInstance<GCArrangedConn> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCArrangedConn", NetClassMask.NetClassGroupGameMask, 0);
        }

        public GameConnection Connection;

        // <GameConnection::onM2cClientRequestedArrangedConnection>[connId: 2][uid:0] Accepting (4->7)
        public uint AcceptFrom = 1; // 88
        public uint AcceptTo = 1; // 92
        public uint Unk3 = 1; // 5; // 96
        public uint Count = 0; // 100
        //public uint LastMsTick = 0; // 104

        // 1 1 0
        /*
            DebugString: "<GameConnection::onM2cClientRequestedArrangedConnection>[connId: 2][uid:0] Accepting (1->1)"
            DebugString: "(2)-------------------------"
            DebugString: "(2)->connectArranged(1->1), false"
            DebugString: "(2)-------------------------"
            DebugString: "Game::addToPeerList / uid : 1 / P2P��\xBC\xD3 \xB4\xEB\xBB\xF3\xC0ڿ\xA1 \xC3߰\xA1"
            DebugString: "RTT : 501,409973"
            ...
            DebugString: "(2)-------------------------"
            DebugString: "<2> P2PConnection::onConnectTerminated (1->1) Timeout-Punch(0)"
            DebugString: "(2)-------------------------"
        */
        /*
            DebugString: "(2)-------------------------"
            DebugString: "<2> P2PConnection::onConnectTerminated (1->1) Timeout-Punch(0)"
            DebugString: "(2)-------------------------"
        */

        // loop
        public uint[] PossIP; // 108 + 8 * i
        public ushort[] PossPort; // 108 + 8 * i + 4

        public string Key = "12345678901234";

        /*
         * GC_ArrangedConn is sent to everyone in the list?
         * 
         */
        public GCArrangedConn() : base(GameNetId.GC_ARRANGED_CONN, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }
        public GCArrangedConn(string key = "") : base(GameNetId.GC_ARRANGED_CONN, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) 
        {
            Key = key;
        }

        public GCArrangedConn(string key, uint ip, ushort port) : base(GameNetId.GC_ARRANGED_CONN, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny)
        {
            Key = key;
            PossIP = new uint[1] { ip };
            PossPort = new ushort[1] { port };
        }
        public GCArrangedConn(string key, uint[] ips, ushort[] ports) : base(GameNetId.GC_ARRANGED_CONN, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny)
        {
            Key = key;
            PossIP = ips;
            PossPort = ports;
        }
        public GCArrangedConn(uint from, uint to, string key, uint[] ips, ushort[] ports) : base(GameNetId.GC_ARRANGED_CONN, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny)
        {
            AcceptFrom = from;
            AcceptTo = to;
            Key = key;
            PossIP = ips;
            PossPort = ports;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            // Note this is arrangement? we need 2 players on this?
            bitStream.Write(AcceptFrom);
            bitStream.Write(AcceptTo);
            bitStream.Write(Unk3);
            //bitStream.Write(LastMsTick);
            bitStream.Write(PossIP.Length);
            for(int i = 0; i < PossIP.Length; i++)
            {
                bitStream.Write(PossIP[i]); // possible DWORD IP
                bitStream.Write(PossPort[i]); // possible PORT
            }

            // 112 bits? - 14 bytes 
            WriteWString(bitStream, Key, (uint)Key.Length); // possible name?
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out AcceptFrom);
            bitStream.Read(out AcceptTo);
            bitStream.Read(out Unk3);
            //bitStream.Read(out LastMsTick);
            bitStream.Read(out Count);
            PossIP = new uint[Count];
            PossPort = new ushort[Count];
            for (int i = 0; i < Count; i++)
            {
                bitStream.Read(out PossIP[i]);
                bitStream.Read(out PossPort[i]);
            }
            WriteWString(bitStream, Key, (uint)Key.Length); // possible name?
        }
        public override void Process(EventConnection ps) { }
    }
}
