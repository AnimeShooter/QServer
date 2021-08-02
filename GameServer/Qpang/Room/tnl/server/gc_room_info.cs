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
    public class GCRoomInfo : GameNetEvent
    {
        private static NetClassRepInstance<GCRoomInfo> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCRoomInfo", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint MasterUid; // 88
        public ushort JoinNum; // 134
        public ushort GameMode; // 136
        public uint GoalPoint; // 140

        public ushort MapNum; // 144
        public uint RoomState; // 148
        public ushort RespawnTime; // 152
        public ushort P2PWaitTime; // 154

        public string Title; // 172

        public byte IsTime; // 156
        public byte Rounds; // 157
        public byte LevelLimit; // 164
        public uint GameId; // 160
        public byte TeamBalance; // 165
        public byte SkillMode; // 166
        public byte PingLevel; // 167
        public byte Melee; // 168
        public byte Event; // 169

        public GCRoomInfo() : base(GameNetId.GC_ROOM_INFO, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCRoomInfo(Room room, bool spectating = false) : base(GameNetId.GC_ROOM_INFO, GuaranteeType.Guaranteed, EventDirection.DirServerToClient) 
        {
            MasterUid = room.MasterId;
            JoinNum = room.MaxPlayers;
            GameMode = (ushort)room.Mode;
            GoalPoint = room.PointsGame ? room.ScorePoints : room.ScoreTime;
            MapNum = room.Map;
            RoomState = room.State;
            RespawnTime = 10 * 1000;
            P2PWaitTime = 6000;
            Title = room.Name;
            IsTime = room.PointsGame ? (byte)0 : (byte)1;
            Rounds = 0;
            LevelLimit = 0;
            GameId = 10; // ???
            TeamBalance = room.TeamSorting ? (byte)1 : (byte)0;
            SkillMode = room.SkillsEnabled ? (byte)1 : (byte)0;
            PingLevel = 0;
            Melee = room.MeleeOnly ? (byte)1 : (byte)0;
            Event = (room.EventRoom || spectating ) ? (byte)1 : (byte)0;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(MasterUid);
            bitStream.Write(JoinNum);
            bitStream.Write(GameMode);
            bitStream.Write(GoalPoint);
            bitStream.Write(MapNum);
            bitStream.Write(RoomState);
            bitStream.Write(RespawnTime);
            bitStream.Write(P2PWaitTime);
            WriteWString(bitStream, Title, 20);
            bitStream.Write(IsTime);
            bitStream.Write(Rounds);
            bitStream.Write(LevelLimit);
            bitStream.Write(GameId);
            bitStream.Write(TeamBalance);
            bitStream.Write(SkillMode);
            bitStream.Write(PingLevel);
            bitStream.Write(Melee);
            bitStream.Write(Event);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)  { }
        public override void Process(EventConnection ps) { }
    }
}
