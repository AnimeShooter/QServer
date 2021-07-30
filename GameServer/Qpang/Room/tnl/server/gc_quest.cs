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
    public class GCQuest : GameNetEvent
    {
        private static NetClassRepInstance<GCQuest> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCQuest", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk01;
        public byte Cmd;
        public ushort Unk02;
        public byte Unk03;

        public GCQuest() : base(GameNetId.GC_QUEST, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(Unk01);
            bitStream.Write(Cmd);
            bitStream.Write(Unk02);
            bitStream.Write(Unk03);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
