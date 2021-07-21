using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNL.Entities;
using TNL.Utils;

namespace Qserver.GameServer.Qpang
{
    public class GCArrangedAccept : GameNetEvent
    {
        private const string key = "123456781234567892345672345672345672345672345678";

        public uint senderId;
        public uint targetId;

        public GCArrangedAccept() : base(GameNetId.GC_ARRANGED_ACCEPT, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public GCArrangedAccept(uint senderId, uint targetId) : base(GameNetId.GC_ARRANGED_ACCEPT, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) 
        {
            this.senderId = senderId;
            this.targetId = targetId;
        }


        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(senderId);
            bitStream.Write(targetId);
            bitStream.Write(1);
            bitStream.Write(1);
            bitStream.Write((uint)222);
            bitStream.Write((ushort)targetId);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
