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
    public class GCHitEssence : GameNetEvent
    {
        private static NetClassRepInstance<GCHitEssence> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCHitEssence", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint PlayerHolderId;
        public uint unk03;
        public float X;
        public float Y;
        public float Z;
        public byte unk07;
        public uint Cmd;
        
        public GCHitEssence() : base(GameNetId.GC_HIT_ESSENCE, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCHitEssence(uint playerId, uint playerHolderId, uint cmd, float x = 0xFF, float y = 0xFF, float z = 0xFF, uint unk03 = 0, uint unk07 = 0) : base(GameNetId.GC_HIT_ESSENCE, GuaranteeType.Guaranteed, EventDirection.DirAny) 
        {
            PlayerId = playerId;
            PlayerHolderId = playerHolderId;
            Cmd = cmd;
            X = x;
            Y = y;
            Z = z;
            this.unk03 = unk03;
            this.unk07 = (byte)unk07;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(PlayerId);
            bitStream.Write(PlayerHolderId);
            bitStream.Write(unk03);
            bitStream.Write(X);
            bitStream.Write(Y);
            bitStream.Write(Z);
            bitStream.Write(unk07);
            bitStream.Write(Cmd);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
