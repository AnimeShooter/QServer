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
    public class GCPvEStart : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEStart> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEStart", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId; // 96
        public uint Unk01 = 5; // 92 - 2, 3, 5, 6
        public uint Unk02; // 100
        public ushort Map; // 104
        public ushort Mode; // 106

        public GCPvEStart() : base(GameNetId.GC_PVE_START, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCPvEStart(Room room, uint playerId) : base(GameNetId.GC_PVE_START, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            PlayerId = playerId;
            Map = room.Map; // 12?
            Mode = (ushort)room.Mode; // 9?
        }
        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(PlayerId);
            bitStream.Write(Unk01);
            bitStream.Write(Unk02);
            bitStream.Write(Map);
            bitStream.Write(Mode);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {

        }
        public override void Process(EventConnection ps) 
        {

        }
    }
}
