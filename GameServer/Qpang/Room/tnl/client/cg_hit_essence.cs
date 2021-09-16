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
    public class CGHitEssence : GameNetEvent
    {
        private static NetClassRepInstance<CGHitEssence> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGHitEssence", NetClassMask.NetClassGroupGameMask, 0);
        }

        public enum Commands : uint
        {
            ESSENCE_THROW =  1,
            ESSENCE_PICK_UP =  2,
            ESSENCE_DROP =  3,
        }

        public uint PlayerId;
        public uint PlayerHolderId;
        public uint unk03;
        public float X;
        public float Y;
        public float Z;
        public byte unk07;
        public uint Cmd;
        public byte unk09;
        public uint unk10;

        public CGHitEssence() : base(GameNetId.CG_HIT_ESSENCE, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out PlayerHolderId);
            bitStream.Read(out unk03);
            bitStream.Read(out X);
            bitStream.Read(out Y);
            bitStream.Read(out Z);
            bitStream.Read(out unk07);
            bitStream.Read(out Cmd);
            bitStream.Read(out unk09);
            bitStream.Read(out unk10);
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }

        public override void Handle(GameConnection conn, Player player)
        {
            var roomPlayer = player.RoomPlayer;
            if (roomPlayer == null || roomPlayer.Spectating)
                return;

            var session = roomPlayer.RoomSessionPlayer;
            if (session == null)
                return;

            var roomSession = session.RoomSession;
            var essenceHolder = roomSession.EssenceHolder;
            if(essenceHolder == null)
            {
                if(Cmd == (uint)Commands.ESSENCE_PICK_UP)
                {
                    if (session.Death)
                        return;

                    roomSession.EssenceHolder = session;
                    var players = roomSession.GetPlayingPlayers();
                    foreach (var p in players) 
                        p.Post(new GCHitEssence(p.Player.PlayerId, player.PlayerId, (uint)Commands.ESSENCE_PICK_UP, X, Y, Z, unk03, unk07));
                }
                else if (Cmd == (uint)Commands.ESSENCE_DROP)
                {
                    roomSession.EssencePosition = new Spawn() { X = X, Y = Y, Z = Z };
                    var players = roomSession.GetPlayingPlayers();
                    foreach (var p in players) 
                        p.Post(new GCHitEssence(p.Player.PlayerId, player.PlayerId, (uint)Commands.ESSENCE_DROP, X, Y, Z, unk03, unk07));
                }
            }
            else
            {
                if (Cmd == (uint)Commands.ESSENCE_THROW && player == essenceHolder.Player)
                {
                    roomSession.EssenceHolder = null;
                    var players = roomSession.GetPlayingPlayers();
                    foreach (var p in players) 
                        p.Post(new GCHitEssence(p.Player.PlayerId, player.PlayerId, (uint)Commands.ESSENCE_THROW, X, Y, Z, unk03, unk07));
                }
            }
        }
    }
}
