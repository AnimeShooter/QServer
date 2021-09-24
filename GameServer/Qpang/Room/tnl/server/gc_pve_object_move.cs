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
    public class GCPvEObjectMove : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEObjectMove> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEObjectMove", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint ObjectId; // 88 // possible vice versa?
        public uint Uid; // 92
        public float X; // 96
        public float Y; // 100
        public float Z; // 104

        public GCPvEObjectMove() : base(GameNetId.GC_PVE_OBJECT_MOVE, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(ObjectId);
            bitStream.Write(Uid);
            bitStream.Write(X);
            bitStream.Write(Y);
            bitStream.Write(Z);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out ObjectId);
            bitStream.Read(out Uid);
            bitStream.Read(out X);
            bitStream.Read(out Y);
            bitStream.Read(out Z);
        }
        public override void Process(EventConnection ps) { }
    }
}
