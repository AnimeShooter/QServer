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
    public class GCPvEItemInit : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEItemInit> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEItemInit", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint ItemId; // 88; // uid
        public uint Uid; // 92; // itemId
        public float X; // x? 96;
        public float Y; // y? 100;
        public float Z; // z? 104;

        public GCPvEItemInit() : base(GameNetId.GC_PVE_ITEM_INIT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCPvEItemInit(uint itemId, uint uid, Position spawn) : base(GameNetId.GC_PVE_ITEM_INIT, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            ItemId = itemId;
            Uid = uid;
            X = spawn.X;
            Y = spawn.Y;
            Z = spawn.Z;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(ItemId);
            bitStream.Write(Uid);
            bitStream.Write(X);
            bitStream.Write(Y);
            bitStream.Write(Z);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out ItemId);
            bitStream.Read(out Uid);
            bitStream.Read(out X);
            bitStream.Read(out Y);
            bitStream.Read(out Z);
        }
        public override void Process(EventConnection ps) { }
    }
}
