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
    public class GCPvEDestroyObject : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEDestroyObject> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEDestroyObject", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Uid; // 88

        public GCPvEDestroyObject() : base(GameNetId.GC_PVE_DESTROY_OBJECT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCPvEDestroyObject(uint uid) : base(GameNetId.GC_PVE_DESTROY_OBJECT, GuaranteeType.Guaranteed, EventDirection.DirAny) 
        {
            Uid = uid;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Uid);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Uid);
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }
    }
}
