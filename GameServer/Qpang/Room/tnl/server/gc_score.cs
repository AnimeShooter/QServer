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

        public List<RoomSessionPlayer> Players = new List<RoomSessionPlayer>();
        public byte Cmd = 0xC8;
        public ushort Unk02;
        public ushort BlueTotallKill;
        public ushort BlueTotalDeath;
        public ushort Unk03;
        public ushort YellowTotalKill;
        public ushort YellowTotalDeath;
        public uint Unk04 = 1;

        public GCScore() : base(GameNetId.GC_SCORE, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }
        public GCScore(List<RoomSessionPlayer> players, RoomSession roomSession, byte cmd) : base(GameNetId.GC_SCORE, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny)
        {
            Players = players;
            Cmd = cmd;
            Unk04 = roomSession.GetElapsedTime();

            if (roomSession.GameMode.IsTeamMode())
            {
                Unk02 = BlueTotallKill = (ushort)roomSession.BluePoints;
                Unk03 = YellowTotalKill = (ushort)roomSession.YellowPoints;
            }
            else
                BlueTotallKill = (ushort)roomSession.GetTopScore();
        }

        public GCScore(RoomSession roomSession, byte cmd) : base(GameNetId.GC_SCORE, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny)
        {
            Cmd = cmd;
            Unk04 = roomSession.GetElapsedTime();

            if (roomSession.GameMode.IsTeamMode())
            {
                Unk02 = BlueTotallKill = (ushort)roomSession.BluePoints;
                Unk03 = YellowTotalKill = (ushort)roomSession.YellowPoints;
            }
            else
                BlueTotallKill = (ushort)roomSession.GetTopScore();
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Cmd);
            bitStream.Write(Unk02);
            bitStream.Write(BlueTotallKill);
            bitStream.Write(BlueTotalDeath);
            bitStream.Write(Unk03);
            bitStream.Write(YellowTotalKill);
            bitStream.Write(YellowTotalDeath);
            bitStream.Write(Unk04);

            byte playerCount = (byte)Players.Count;
            if (playerCount > 16)
                playerCount = 16;

            bitStream.Write(playerCount);

            byte team0Spot = 1;
            byte team1Spot = 1;
            byte team2Spot = 1; // ??

            byte playerIndex = 0;

            foreach(var player in Players)
            {
                var actPlayer = player.Player;

                bitStream.Write(actPlayer.PlayerId);
                bitStream.Write((ushort)actPlayer.Level);
                bitStream.Write((byte)4);

                bitStream.Write((ushort)player.Score); // essence
                bitStream.Write((ushort)player.Kills);
                bitStream.Write((ushort)player.Deaths);
                bitStream.Write((ushort)player.Score);
                bitStream.Write(player.Team);

                WriteWString(bitStream, actPlayer.Name, 16);
                bitStream.Write((uint)player.PreyScore); // Prey score?

                playerIndex++;
                if (playerIndex >= 16)
                    break;
            }
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
