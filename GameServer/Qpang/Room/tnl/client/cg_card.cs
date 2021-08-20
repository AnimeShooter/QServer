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
    public class CGCard : GameNetEvent
    {
        private static NetClassRepInstance<CGCard> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGCard", NetClassMask.NetClassGroupGameMask, 0);
        }
        public CGCard() : base(GameNetId.CG_CARD, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }

        public enum Commands
        {
            ABILITY = 0x07,
            USE_CARD = 0x09
        };

        public uint Uid;
        public uint TargetUid;
        public uint Cmd;
        public uint CardType;
        public uint ItemId;
        public ulong SeqId;

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Uid);
            bitStream.Read(out TargetUid);
            bitStream.Read(out Cmd);
            bitStream.Read(out CardType);
            bitStream.Read(out ItemId);
            bitStream.Read(out SeqId);
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }

        public override void Handle(GameConnection conn, Player player)
        {
            var roomPlayer = player.RoomPlayer;
            if (roomPlayer == null)
                return;

            var roomSession = roomPlayer.Room.RoomSession;
            if (roomSession == null)
                return;

            // TODO: handle skill usage (subtract points?)

            // CMD
            /*
             * 4 - start
             * 9 - stop
             * 
             * id:
             * roll - 1409286147 (type 7)
             * 54000001 ken special
             * 54000002 death
             * 54000003 roll
             * 54000004 left roll
             * 54000005 right roll
             * 54000006 double jump 
             * 54000007 kuma special
             * 
             * Assasin, targget self, type 9, id i:1258356765, seqId:1
             */

            Console.WriteLine($"ActionType: {Cmd}, t:{TargetUid} ct:{CardType}, i:{ItemId}, s:{SeqId}");

            // check CMD?
            //Game.Instance.SkillManager.GetSkill()

            roomSession.RelayPlaying<GCCard>(Uid, TargetUid, (byte)Cmd, CardType, ItemId, SeqId);
        }
    }
}
