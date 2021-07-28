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
    public class CGMapObject : GameNetEvent
    {
        private static NetClassRepInstance<CGMapObject> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGMapObject", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint unk01;
        public uint unk02;
        public uint unk03;
        public uint unk04;
        public uint unk05;
        public uint unk06;
        public uint unk07;
        public uint unk08;

        public CGMapObject() : base(GameNetId.CG_MAPOBJECT, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out unk01);
            bitStream.Read(out unk02);
            bitStream.Read(out unk03);
            bitStream.Read(out unk04);
            bitStream.Read(out unk05);
            bitStream.Read(out unk06);
            bitStream.Read(out unk07);
            bitStream.Read(out unk08);
        }
        public override void Process(EventConnection ps) { }
    }
}
