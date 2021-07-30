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
    public class GCMapObject : GameNetEvent
    {
        private static NetClassRepInstance<GCMapObject> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCMapObject", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk01;
        public uint Unk02;
        public uint Unk03;
        public uint Unk04;
        public uint Unk05;
        public uint Unk06;
        public uint Unk07;
        public uint Unk08;

        public GCMapObject() : base(GameNetId.GC_MAPOBJECT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

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
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
