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
    public class GCArrangedAccept : GameNetEvent
    {
        private static NetClassRepInstance<GCArrangedAccept> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCArrangedAccept", NetClassMask.NetClassGroupGameMask, 0);
        }

        private const string key = "123456781234567892345672345672345672345672345678";

        public uint SenderId;
        public uint TargetId;

        public GCArrangedAccept() : base(GameNetId.GC_ARRANGED_ACCEPT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public GCArrangedAccept(uint senderId, uint targetId) : base(GameNetId.GC_ARRANGED_ACCEPT, GuaranteeType.Guaranteed, EventDirection.DirAny) 
        {
            SenderId = senderId;
            TargetId = targetId;
        }


        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(SenderId);
            bitStream.Write(TargetId);
            bitStream.Write(1);
            bitStream.Write(1);
            bitStream.Write((uint)222);
            bitStream.Write((ushort)TargetId);
            bitStream.WriteString(key, (byte)key.Length);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
