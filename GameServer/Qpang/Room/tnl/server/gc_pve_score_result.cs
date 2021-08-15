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
    public class GCPvEScoreResult : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEScoreResult> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEScoreResult", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk1; // 88
        public uint Unk2; // 92
        public uint Unk3; // 96
        public uint Unk4; // 100
        public uint Unk5; // 104
        public uint Unk6; // 108
        public uint Unk7; // 112
        public uint Unk8; // 116
        public uint Unk9; // 120
        public uint Unk10; // 124
        public uint Unk11; // 128
        public uint Unk12; // 132
        public uint Unk13; // 136
        public uint Unk14; // 140
        public uint Unk15; // 144
        public byte Unk16; // 146
        public ushort Unk17; // 148
        
        public GCPvEScoreResult() : base(GameNetId.GC_PVE_SCORE_RESULT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Unk1);
            bitStream.Write(Unk2);
            bitStream.Write(Unk3);
            bitStream.Write(Unk4);
            bitStream.Write(Unk5);
            bitStream.Write(Unk6);
            bitStream.Write(Unk7);
            bitStream.Write(Unk8);
            bitStream.Write(Unk9);
            bitStream.Write(Unk10);
            bitStream.Write(Unk11);
            bitStream.Write(Unk12);
            bitStream.Write(Unk13);
            bitStream.Write(Unk14);
            bitStream.Write(Unk15);
            bitStream.Write(Unk16);
            bitStream.Write(Unk17);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out Unk1);
            bitStream.Read(out Unk2);
            bitStream.Read(out Unk3);
            bitStream.Read(out Unk4);
            bitStream.Read(out Unk5);
            bitStream.Read(out Unk6);
            bitStream.Read(out Unk7);
            bitStream.Read(out Unk8);
            bitStream.Read(out Unk9);
            bitStream.Read(out Unk10);
            bitStream.Read(out Unk11);
            bitStream.Read(out Unk12);
            bitStream.Read(out Unk13);
            bitStream.Read(out Unk14);
            bitStream.Read(out Unk15);
            bitStream.Read(out Unk16);
            bitStream.Read(out Unk17);
        }
        public override void Process(EventConnection ps) 
        {
            Post(ps);
        }
    }
}
