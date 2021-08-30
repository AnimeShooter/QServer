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
    public class GCPvEEventObject : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEEventObject> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEEventObject", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Uid;
        public bool Triggerd;

        public GCPvEEventObject() : base(GameNetId.GC_PVE_EVENT_OBJECT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCPvEEventObject(uint uid, bool trigger) : base(GameNetId.GC_PVE_EVENT_OBJECT, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            Uid = uid;
            Triggerd = trigger;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Uid);
            bitStream.Write(Triggerd);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out Uid);
            bitStream.Read(out Triggerd);
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }
    }
}
