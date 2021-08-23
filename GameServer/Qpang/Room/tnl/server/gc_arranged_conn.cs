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

        public uint Unk1; // 88
        public uint Unk2; // 92
        public uint Unk3; // 96
        public uint Count = 0; // 100

        // loop
        public uint[] Unk5; // 108 + 8 * v7
        public uint[] Unk6; // 108 + 8 * v7 + 4

        public string UnkString;

        /*
         * GC_ArrangedConn is sent to everyone in the list?
         * 
         */

        public GCArrangedConn() : base(GameNetId.GC_ARRANGED_CONN, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Unk1);
            bitStream.Write(Unk2);
            bitStream.Write(Unk3);
            bitStream.Write(Count);
            for(int i = 0; i < Count; i++)
            {
                bitStream.Write(Unk5[i]); // possible PlayerUid
                bitStream.Write(Unk6[i]); // possible TargetUid
            }
            WriteWString(bitStream, GCArrangedAccept.Key, (uint)GCArrangedAccept.Key.Length); // possible name?
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Unk1);
            bitStream.Read(out Unk2);
            bitStream.Read(out Unk3);
            bitStream.Read(out Count);
            Unk5 = new uint[Count];
            Unk6 = new uint[Count];
            for (int i = 0; i < Count; i++)
            {
                bitStream.Read(out Unk5[i]);
                bitStream.Read(out Unk6[i]);
            }
            WriteWString(bitStream, GCArrangedAccept.Key, (uint)GCArrangedAccept.Key.Length); // possible name?
        }
        public override void Process(EventConnection ps) { }
    }
}
