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
    public class GCPvEHitN2N : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEHitN2N> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEHitN2N", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk1; // 88
        public uint Unk2; // 92
        public uint Unk3; // 96
        public uint Unk4; // 100
        public uint Unk5; // 104
        public uint Unk6; // 108
        public uint Unk7; // 112
        public uint Unk8; // 116
        public byte Unk9; // 120
        public byte Unk10; // 121
        //public word Unk11; //
        
        public GCPvEHitN2N() : base(GameNetId.GC_PVE_HIT_N2N, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

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
        }
        public override void Process(EventConnection ps) 
        {

        }
    }
}
