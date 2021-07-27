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
    public class CCCharm : GameNetEvent
    {
        private static NetClassRepInstance<CCCharm> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CCCharm", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint unk01;
        public uint unk02;
        public uint unk03;
        public uint unk04;
        public uint unk05;

        public CCCharm() : base(GameNetId.CC_CHARM, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out unk01);
            bitStream.Read(out unk02);
            bitStream.Read(out unk03);
            bitStream.Read(out unk04);
            bitStream.Read(out unk05);
        }
        public override void Process(EventConnection ps) { }
    }
}
