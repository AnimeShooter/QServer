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
    public class GCPvEHitNpc : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEHitNpc> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEHitNpc", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk1; // 140
        public uint Unk2; // 144
        public uint Unk3; // 88
        public uint Unk4; // 116
        public uint Unk5; // 120
        public uint Unk6; // 124
        public uint Unk7; // 128
        public uint Unk8; // 132
        public uint Unk9; // 136
        public uint Unk10; // 92
        public byte Unk11; // 148
        public byte Unk12; // 149
        public uint Unk13; // 96
        public ulong Unk14; // 104
        public byte Unk15; // 112
        public byte Unk16; // 150
        public byte Unk17; // 151
        public uint Unk18; // 152
        public ushort Unk19; // 156
        public byte Unk20; // 158
        public uint Unk21; // 160


        public GCPvEHitNpc() : base(GameNetId.GC_PVE_HIT_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

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
            bitStream.Write(Unk18);
            bitStream.Write(Unk19);
            bitStream.Write(Unk20);
            bitStream.Write(Unk21);
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
            bitStream.Read(out Unk18);
            bitStream.Read(out Unk19);
            bitStream.Read(out Unk20);
            //bitStream.Read(out Unk21); // only on write?
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }
    }
}
