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
    public class GCPvEObjectInit : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEObjectInit> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEObjectInit", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk1; // 88 Uid?
        public uint Unk2; // 92 ObjectId?
        public float X; // x? 96
        public float Y; // y? 100
        public float Z; // z? 104
        public ushort Unk6; // 108 State?

        public GCPvEObjectInit() : base(GameNetId.GC_PVE_OBJECT_INIT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCPvEObjectInit(uint objId, uint uid, Spawn spawn, ushort unk6) : base(GameNetId.GC_PVE_OBJECT_INIT, GuaranteeType.Guaranteed, EventDirection.DirAny) 
        {
            Unk1 = objId;
            Unk2 = uid;
            X = spawn.X;
            Y = spawn.Y;
            Z = spawn.Z;
            Unk6 = unk6;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Unk1);
            bitStream.Write(Unk2);
            bitStream.Write(X);
            bitStream.Write(Y);
            bitStream.Write(Z);
            bitStream.Write(Unk6);
        }
        public override  void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Unk1);
            bitStream.Read(out Unk2);
            bitStream.Read(out X);
            bitStream.Read(out Y);
            bitStream.Read(out Z);
            bitStream.Read(out Unk6);
        }
        public override void Process(EventConnection ps) { }
    }
}
