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
    public class GCMotion : GameNetEvent
    {
        private static NetClassRepInstance<GCMotion> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCMotion", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Cmd;
        public uint Unk02;
        public uint Unk03;
        public uint Unk04;
        public uint Unk05;
        public uint Unk06;
        public uint Unk07;
        public uint Unk08;
        public uint Unk09;
        public uint PlayerId;
        public uint IsP2P;

        public GCMotion() : base(GameNetId.GC_MOTION, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCMotion(uint cmd, uint unk02, uint unk03, uint unk04, uint unk05, uint unk06, uint unk07, uint unk08, uint unk09, uint playerId, byte isP2P = 0) : base(GameNetId.GC_MOTION, GuaranteeType.Guaranteed, EventDirection.DirServerToClient)
        {
            Cmd = cmd;
            Unk02 = unk02;
            Unk03 = unk03;
            Unk04 = unk04;
            Unk05 = unk05;
            Unk06 = unk06;
            Unk07 = unk07;
            Unk08 = unk08;
            Unk09 = unk09;
            PlayerId = playerId;
            IsP2P = (uint)isP2P;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Cmd);
            bitStream.Write(Unk02);
            bitStream.Write(Unk03);
            bitStream.Write(Unk04);
            bitStream.Write(Unk05);
            bitStream.Write(Unk06);
            bitStream.Write(Unk07);
            bitStream.Write(Unk08);
            bitStream.Write(Unk09);
            bitStream.Write(PlayerId);
            bitStream.Write(IsP2P);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
