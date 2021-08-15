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
    public class GCPvEStart : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEStart> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEStart", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk1; // 96
        public uint Unk2; // 92
        public uint Unk3; // 100
        public ushort Unk4; // 104
        public ushort Unk5; // 106

        public GCPvEStart() : base(GameNetId.GC_PVE_START, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(Unk1);
            bitStream.Write(Unk2);
            bitStream.Write(Unk3);
            bitStream.Write(Unk4);
            bitStream.Write(Unk5);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Unk1);
            bitStream.Read(out Unk2);
            bitStream.Read(out Unk3);
            bitStream.Read(out Unk4);
            bitStream.Read(out Unk5);
        }
        public override void Process(EventConnection ps) 
        {
            Post(ps);
        }
    }
}
