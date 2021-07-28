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
    public class CGTarget : GameNetEvent
    {
        private static NetClassRepInstance<CGTarget> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public uint Cmd;
        public uint PlayerId;
        public uint TargetId;

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGTarget", NetClassMask.NetClassGroupGameMask, 0);
        }
        public CGTarget() : base(GameNetId.CG_TARGET, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Cmd);
            bitStream.Read(out PlayerId);
            bitStream.Read(out TargetId);
        }
        public override void Process(EventConnection ps) 
        { 
            // Room  OnplayerTarget
        }
    }
}
