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
        public uint Uid; // 92
        public float X; // 96
        public float Y; // 100
        public float Z; // 104
        public byte Unk6; // 108

        public GCPvEAreaTriggerInit() : base(GameNetId.GC_PVE_AREA_TRIGGER_INIT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCPvEAreaTriggerInit(uint id, uint uid, Position spawn, byte unk6) : base(GameNetId.GC_PVE_AREA_TRIGGER_INIT, GuaranteeType.Guaranteed, EventDirection.DirAny) 
        {
            TriggerId = id;
            Uid = uid;
            X = spawn.X;
            Y = spawn.Y;
            Z = spawn.Z;
            Unk6 = unk6;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(TriggerId);
            bitStream.Write(Uid);
            bitStream.Write(X);
            bitStream.Write(Y);
            bitStream.Write(Z);
            bitStream.Write(Unk6);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out TriggerId);
            bitStream.Read(out Uid);
            bitStream.Read(out X);
            bitStream.Read(out Y);
            bitStream.Read(out Z);
            bitStream.Read(out Unk6);
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }
    }
}
