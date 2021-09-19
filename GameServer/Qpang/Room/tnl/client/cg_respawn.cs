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
    public class CGRespawn : GameNetEvent
    {
        private static NetClassRepInstance<CGRespawn> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGRespawn", NetClassMask.NetClassGroupGameMask, 0);
        }

        public enum Commands : uint
        {
            UNK1 = 1, // GetSpawnPos + BroadcastSpawnPos
            ME = 2, //  ServerGame::broadcastPotalPos Portal*?
            UNK3 = 3,
            OTHER = 4,
            DEFAULT = 5, // similar to OTHER?
        }

        public uint unk01;
        public byte unk02;
        public uint unk03;

        public CGRespawn() : base(GameNetId.CG_RESPAWN, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out unk01);
            bitStream.Read(out unk02);
            bitStream.Read(out unk03);
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

            var session = roomPlayer.RoomSessionPlayer;
            if (session == null || session.Death)
                return;

            var spawn = Game.Instance.SpawnManager.GetRandomTeleportSpawn(roomPlayer.Room.Map);
            var roomSession = session.RoomSession;
            roomSession.RelayPlaying<GCRespawn>(player.PlayerId, session.Character, (uint)2, spawn.X, spawn.Y, spawn.Z, false);
        }
    }
}
