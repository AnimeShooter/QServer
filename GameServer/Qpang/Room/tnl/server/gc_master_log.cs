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
    public class GCMasterLog : GameNetEvent
    {
        private static NetClassRepInstance<GCMasterLog> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCMasterLog", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk01;
        public uint Unk02;
        public uint Unk03;
        public ushort Unk04;
        public ushort Unk05;
        public byte Unk06;
        public uint Unk07;
        public uint Unk08;
        public uint Unk09;
        public uint Unk10;
        public uint Unk11;
        public uint Unk12;
        public byte Unk13;
        public ByteBuffer Buffer = new ByteBuffer(); // wuth ??

        public GCMasterLog() : base(GameNetId.GC_MASTERLOG, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Unk01);
            bitStream.Write(Unk02);
            bitStream.Write(Unk03);
            bitStream.Write(Unk04);
            bitStream.Write(Unk05);
            bitStream.Write(Unk06);
            bitStream.Write(Unk07);
            bitStream.Write(Unk08);
            bitStream.Write(Unk09);
            bitStream.Write(Unk10);
            bitStream.Write(Unk11);
            bitStream.Write(Unk12);
            bitStream.Write(Unk13);
            bitStream.Write(Buffer);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
