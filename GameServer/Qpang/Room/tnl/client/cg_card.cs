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

        public enum Commands // types
        {
            ABILITY = 0x07,
            USE_CARD = 0x09
            // 8 octo?
            // 4  ServerPlayer::applySkill target
        };

        public uint Uid;
        public uint TargetUid;
        public uint Cmd;
        public uint CardType; // !1 && !4 = !inv_card; !9 = !game_card
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
            if (player == null)
                return;

            var roomPlayer = player.RoomPlayer;
            if (roomPlayer == null)
                return;

            var roomSession = roomPlayer.Room.RoomSession;
            if (roomSession == null)
                return;

            var activeSkill = roomPlayer.RoomSessionPlayer.SkillManager.ActiveSkillCard;
            if (activeSkill != null)
            {
                switch ((Items)activeSkill.Id)
                {
                    case Items.SKILL_CAMO:
                        roomPlayer.RoomSessionPlayer.SkillManager.DisableSkill(); // consume but allow skill usage
                        break;
                    case Items.SKILL_IRONWALL:
                    case Items.SKILL_IRONWALL2:
                    default:
                        return; // prevent skill usage
                }
            }

            // TODO: handle skill usage (subtract points?)

            // CMD
            /*
             * 4 - start
             * 8 - add skill points 
             * 9 - stop
             * (TODO: set skillpoints?)
             * 
             * id:
             * (type 7, character)
             * 54000001 ken special
             * 54000002 death
             * 54000003 roll
             * 54000004 left roll
             * 54000005 right roll
             * 54000006 double jump 
             * 54000007 kuma special
             * 
             * (type 9, cards)
             * 
             * Assasin, targget self, type 9, id i:1258356765, seqId:1
             */

            Console.WriteLine($"ActionType: {Cmd}, t:{TargetUid} ct:{CardType}, i:{ItemId}, s:{SeqId}");

            // check CMD?
            //Game.Instance.SkillManager.GetSkill()

            roomPlayer.RoomSessionPlayer.SkillManager.UseSkill(Uid, TargetUid, (byte)Cmd, CardType, ItemId, SeqId);
        }
    }
}
