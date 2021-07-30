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
    public class GCStart : GameNetEvent
    {
        private static NetClassRepInstance<GCStart> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCStart", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint Unk01 = 2;
        public uint Unk02;
        public ushort Map;
        public ushort Mode;

        public GCStart() : base(GameNetId.GC_START, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCStart(Room room, uint playerId) : base(GameNetId.GC_START, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            PlayerId = playerId;
            Map = room.Map;
            Mode = (ushort)room.Mode;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(PlayerId);
            bitStream.Write(Unk01);
            bitStream.Write(Unk02);
            bitStream.Write(Map);
            bitStream.Write(Mode);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
