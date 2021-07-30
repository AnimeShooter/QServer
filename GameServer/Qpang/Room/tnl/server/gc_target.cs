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
    public class GCTarget : GameNetEvent
    {
        private static NetClassRepInstance<GCTarget> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCTarget", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Cmd;
        public uint PlayerId;
        public uint TargetId;

        public GCTarget() : base(GameNetId.GC_TARGET, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(Cmd);
            bitStream.Write(PlayerId);
            bitStream.Write(TargetId);
            bitStream.Write((uint)0); // 1 for crash ;D
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
