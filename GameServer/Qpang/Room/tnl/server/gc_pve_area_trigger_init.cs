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
    public class GCPvEAreaTriggerInit : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEAreaTriggerInit> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEAreaTriggerInit", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint TriggerId; // 88
        public float minBoundX; // 92
        public float minBoundY; // 96
        public float maxBoundX; // 100
        public float maxBoundY; // 104
        public byte cmd = 1; // 108 - 1: create area

        public GCPvEAreaTriggerInit() : base(GameNetId.GC_PVE_AREA_TRIGGER_INIT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCPvEAreaTriggerInit(uint id, float uid, /* TODO boudry*/ Position spawn, byte unk6) : base(GameNetId.GC_PVE_AREA_TRIGGER_INIT, GuaranteeType.Guaranteed, EventDirection.DirAny) 
        {
            TriggerId = id;
            minBoundX = uid;
            minBoundY = 0.0f;
            minBoundX = 0.0f;
            maxBoundX = 0.0f;
            maxBoundY = 0.0f;
            cmd = unk6;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(TriggerId);
            bitStream.Write(minBoundX);
            bitStream.Write(minBoundY);
            bitStream.Write(maxBoundX);
            bitStream.Write(maxBoundY);
            bitStream.Write(cmd);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out TriggerId);
            bitStream.Read(out minBoundX);
            bitStream.Read(out minBoundY);
            bitStream.Read(out maxBoundX);
            bitStream.Read(out maxBoundY);
            bitStream.Read(out cmd);
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }
    }
}
