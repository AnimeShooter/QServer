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
    public class GCSync : GameNetEvent
    {
        private static NetClassRepInstance<GCSync> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCSync", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Cmd;
        public uint PlayerId;
        public uint EntityId;
        public float PosX;
        public float PosY;
        public float PosZ;
        public byte IsP2P;

        public GCSync() : base(GameNetId.GC_SYNC, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public GCSync(uint cmd, uint playerId, uint entityId, float x, float  y, float z, byte isP2P = 0) : base(GameNetId.GC_SYNC, GuaranteeType.Guaranteed, EventDirection.DirServerToClient)
        {
            Cmd = cmd;
            PlayerId = playerId;
            EntityId = entityId;
            PosX = x;
            PosY = y;
            PosZ = z;
            IsP2P = isP2P;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Cmd);
            bitStream.Write(PlayerId);
            bitStream.Write(EntityId);
            bitStream.Write(PosX);
            bitStream.Write(PosY);
            bitStream.Write(PosZ);
            bitStream.Write(IsP2P);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
