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
    public class GCReady : GameNetEvent
    {
        private static NetClassRepInstance<GCReady> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCReady", NetClassMask.NetClassGroupGameMask, 0);
        }

        public ulong PlayerId;
        public byte Cmd;

        public GCReady() : base(GameNetId.GC_READY, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public GCReady(uint playerId, uint cmd) : base(GameNetId.GC_READY, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            PlayerId = playerId;
            Cmd = (byte)cmd;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(PlayerId);
            bitStream.Write(Cmd);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
