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
    public class GCGameState : GameNetEvent
    {
        private static NetClassRepInstance<GCGameState> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCGameState", NetClassMask.NetClassGroupGameMask, 0);
        }

        public struct BonusInfo
        {
            public ushort Id;
            public float Exp;
            public float Don;
            public byte Op;
            public byte type;
        }

        public uint Uid;
        public byte Cmd;
        public uint Data;
        public uint DataUid;
        public ushort GainExp;
        public ushort GainDon;
        public byte Counter;
        public List<uint> GainAchievements = new List<uint>();
        public byte BonusCount = 0;
        public List<uint> BonusList = new List<uint>();


        public GCGameState() : base(GameNetId.GC_GAME_STATE, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public GCGameState(uint playerId, uint cmd, uint value = 0, uint dataUid = 0) : base(GameNetId.GC_GAME_STATE, GuaranteeType.Guaranteed, EventDirection.DirServerToClient)
        {
            Uid = playerId;
            Cmd = (byte)cmd;
            Data = value;
            DataUid = dataUid;
        }

        public GCGameState(RoomSessionPlayer player, uint cmd, uint value = 0, uint dataUid = 0) : base(GameNetId.GC_GAME_STATE, GuaranteeType.Guaranteed, EventDirection.DirServerToClient) 
        {
            Uid = player.Player.PlayerId;
            Cmd = (byte)cmd;
            Data = value;
            DataUid = dataUid;

            GainExp = (ushort)player.GetExperience();
            GainDon = (ushort)player.GetDon();

            this.GainAchievements = player.Player.AchievementContainer.ListRecent;
        }


        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(Uid);
            bitStream.Write(Cmd);
            bitStream.Write(Data);
            bitStream.Write(DataUid);
            bitStream.Write(GainExp);
            bitStream.Write(GainDon);
            bitStream.Write((byte)GainAchievements.Count);

            foreach (var achi in GainAchievements)
                bitStream.Write((ushort)achi);

            bitStream.Write((byte)BonusCount);

            // TODO: write BonusInfo
            for (int i = 0; i < BonusCount; i++) // ???
            {
                bitStream.Write((ushort)1);
                bitStream.Write((float)1234);
                bitStream.Write((float)2345); // debug and find out?
                bitStream.Write((byte)12);
                bitStream.Write((byte)1);
            }

        }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
