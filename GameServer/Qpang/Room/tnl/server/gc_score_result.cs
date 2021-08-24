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
    public class GCScoreResult : GameNetEvent
    {
        private static NetClassRepInstance<GCScoreResult> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCScoreResult", NetClassMask.NetClassGroupGameMask, 0);
        }

        public List<RoomSessionPlayer> Players;
        public ushort BlueScore;
        public ushort BlueKills;
        public ushort BlueDeaths;
        public ushort YellowScore;
        public ushort YellowSkills;
        public ushort YellowDeaths;

        public GCScoreResult() : base(GameNetId.GC_SCORE_RESULT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCScoreResult(RoomSession roomSession, List<RoomSessionPlayer> players) : base(GameNetId.GC_SCORE_RESULT, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            Players = players;
            if (roomSession.GameMode.IsTeamMode())
            {
                BlueScore = BlueKills = (ushort)roomSession.BluePoints;
                YellowScore = YellowScore = (ushort)roomSession.YellowPoints;
            }
            else
                BlueScore = (ushort)roomSession.GetTopScore();
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(BlueScore);
            bitStream.Write(BlueKills);
            bitStream.Write(BlueDeaths);
            bitStream.Write(YellowScore);
            bitStream.Write(YellowSkills);
            bitStream.Write(YellowDeaths);
            bitStream.Write((ushort)0);
            bitStream.Write((ushort)0);
            bitStream.Write((ushort)0);
            bitStream.Write((ushort)0);
            bitStream.Write((ushort)0);
            bitStream.Write((ushort)0);

            byte playerCount = (byte)Players.Count;
            if (playerCount > 16) // NOTE: also crash on team count?
                playerCount = 16;

            bitStream.Write(playerCount);
            bitStream.Write((uint)0);

            foreach(var player in Players)
            {
                byte playerIndex = 0;

                bitStream.Write(player.Player.PlayerId);
                bitStream.Write((ushort)0); // Unk02
                bitStream.Write((byte)0); // Unk03
                bitStream.Write(player.Team);
                bitStream.Write(player.Score);
                bitStream.Write(player.Kills);
                bitStream.Write(player.Deaths);
                bitStream.Write(player.Score); // doesnt mattter?
                bitStream.Write(player.GetDon());
                bitStream.Write(player.GetExperience());
                bitStream.Write((uint)0); // weird B icon -> Bonusus?
                bitStream.Write(player.Player.AchievementContainer.ListRecent.Count == 0 ? (byte)0 : (byte)1); // has echiev
                bitStream.Write((byte)player.EventItemPickUps);
                bitStream.Write((byte)0); // Unk14
                bitStream.Write((byte)0); // Unk15

                bool hasXPBonus = player.ExpRate > 0;
                bool hasDonBonus = player.DonRate > 0;

                bitStream.Write(hasDonBonus && hasXPBonus ? (byte)3 : hasDonBonus ? (byte)2 : hasXPBonus ? (byte)1 : (byte)0); // Unk16 = 1: exp only, 2: don only, 3: both
                bitStream.Write(player.DonRate);
                bitStream.Write(player.ExpRate);

                // test
                WriteWString(bitStream, player.Player.Name, 16); // TODO check
                bitStream.Write((uint)0); // Unk19

                playerIndex++;
                if (playerIndex >= 16)
                    break;
            }

        }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
