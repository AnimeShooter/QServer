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
    public class GCScore : GameNetEvent
    {
        private static NetClassRepInstance<GCScore> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCScore", NetClassMask.NetClassGroupGameMask, 0);
        }

        public List<RoomSessionPlayer> Players;
        public byte Cmd = 0xC8;
        public ushort Unk02;
        public ushort BlueTotallKill;
        public ushort BlueTotalDeath;
        public ushort Unk03;
        public ushort YellowTotalKill;
        public ushort YellowTotalDeath;
        public uint Unk04 = 1;

        public GCScore() : base(GameNetId.GC_SCORE, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }
        //public GCScore(List<RoomSessionPlayer> players, RoomSession roomSession, byte cmd) : base(GameNetId.GC_SCORE, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) 
        //{
        //    Players = players;
        //    Cmd = cmd;
        //    Unk04 = roomSession.GetElapsedTime();

        //    if (roomSession.GameMode.IsTeamMode())
        //    {
        //        Unk02 = BlueTotallKill = roomSession.BluePouints
        //    }
        //    else
        //        BlueTotallKill = (ushort)roomSession.GetTopScore();
        //}
        //public GCScore() : base(GameNetId.GC_SCORE, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
