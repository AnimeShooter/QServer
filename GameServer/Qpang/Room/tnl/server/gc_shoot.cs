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
    public class GCShoot : GameNetEvent
    {
        private static NetClassRepInstance<GCShoot> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCShoot", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint ItemId;
        public uint Unk03;
        public float SrcX;
        public float SrcY;
        public float SrcZ;
        public float DirX;
        public float DirY;
        public float DirZ;
        public uint EntityId;
        public uint IsP2P;

        public GCShoot() : base(GameNetId.GC_SHOOT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCShoot(uint playerId, uint itemId, float srcX, float srcY, float srcZ, float dirX, float dirY, float dirZ, uint entityId, uint isP2P) : base(GameNetId.GC_SHOOT, GuaranteeType.Guaranteed, EventDirection.DirServerToClient)
        {
            PlayerId = playerId;
            ItemId = itemId;
            Unk03 = 0xFFFFFFFF;
            SrcX = srcX;
            SrcY = srcY;
            SrcZ = srcZ;
            DirX = dirX;
            DirY = dirY;
            DirZ = dirZ;
            EntityId = entityId;
            IsP2P = isP2P;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(PlayerId);
            bitStream.Write(ItemId);
            bitStream.Write(Unk03);
            bitStream.Write(SrcX);
            bitStream.Write(SrcY);
            bitStream.Write(SrcZ);
            bitStream.Write(DirX);
            bitStream.Write(DirY);
            bitStream.Write(DirZ);
            bitStream.Write(EntityId);
            bitStream.Write(IsP2P);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
