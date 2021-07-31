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
    public class GCExit : GameNetEvent
    {
        private static NetClassRepInstance<GCExit> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCExit", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint Cmd;
        public uint MasterId;
        public uint unk01 = 0;

        public GCExit() : base(GameNetId.GC_EXIT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCExit(uint playerId, uint cmd, uint masterId) : base(GameNetId.GC_EXIT, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            PlayerId = playerId;
            Cmd = cmd;
            MasterId = masterId;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(PlayerId);
            bitStream.Write(Cmd);
            bitStream.Write(MasterId);
            bitStream.Write(unk01);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
