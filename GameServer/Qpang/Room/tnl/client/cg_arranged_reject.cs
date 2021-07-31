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
    public class CGArrangedReject : GameNetEvent
    {
        private static NetClassRepInstance<CGArrangedReject> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGArrangedReject", NetClassMask.NetClassGroupGameMask, 0);
        }
        public CGArrangedReject() : base(GameNetId.CG_ARRANGED_REJECT, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer)
        {
            unk04 = new ByteBuffer();
        }

        public uint SourcePlayerId;
        public uint TargetPlayerId;
        public uint unk03;
        public ByteBuffer unk04;

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out SourcePlayerId);
            bitStream.Read(out TargetPlayerId);
            bitStream.Read(out unk03);
            bitStream.Read(unk04);
        }
        public override void Process(EventConnection ps) { }
    }
}
