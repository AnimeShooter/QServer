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
    public class GCMove : GameNetEvent
    {
        private static NetClassRepInstance<GCMove> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCMove", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint Cmd;
        public float Unk01;
        public float Unk02;
        public float Unk03;
        public float Unk04;
        public float Unk05;
        public float Unk06;
        public float Unk07;
        public float Unk08;
        public float Unk09;
        public float Unk10;
        public byte IsP2P = 0;

        public GCMove() : base(GameNetId.GC_MOVE, GuaranteeType.Unguaranteed, EventDirection.DirServerToClient) { }
        public GCMove(uint playerId, uint cmd, float unk01, float unk02, float unk03, float unk04, float unk05, float unk06, float unk07, float unk08, float unk09, float unk10) : base(GameNetId.GC_MOVE, GuaranteeType.Unguaranteed, EventDirection.DirServerToClient)
        {
            PlayerId = playerId;
            Cmd = cmd;
            Unk01 = unk01;
            Unk02 = unk02;
            Unk03 = unk03;
            Unk04 = unk04;
            Unk05 = unk05;
            Unk06 = unk06;
            Unk07 = unk07;
            Unk08 = unk08;
            Unk09 = unk09;
            Unk10 = unk10;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(PlayerId);
            bitStream.Write(Cmd);
            bitStream.Write(Unk01);
            bitStream.Write(Unk02);
            bitStream.Write(Unk03);
            bitStream.Write(Unk04);
            bitStream.Write(Unk05);
            bitStream.Write(Unk06);
            bitStream.Write(Unk07);
            bitStream.Write(Unk08);
            bitStream.Write(Unk09);
            bitStream.Write(Unk10);
            bitStream.Write(IsP2P);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
