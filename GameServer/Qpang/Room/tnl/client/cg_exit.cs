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
    public class CGExit : GameNetEvent
    {
        private static NetClassRepInstance<CGExit> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGExit", NetClassMask.NetClassGroupGameMask, 0);
        }

        public enum Commands : byte
        {
            LEAVE = 0,
            KICK = 1,
            MASTER_KICK_IDLE = 2
        }

        public uint PlayerId;
        public byte Cmd;

        public CGExit() : base(GameNetId.CG_EXIT, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }

        public CGExit(uint playerId, byte cmd) : base(GameNetId.CG_EXIT, GuaranteeType.Guaranteed, EventDirection.DirClientToServer)
        {
            PlayerId = playerId;
            Cmd = cmd;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(PlayerId);
            bitStream.Write(Cmd);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out Cmd);
        }
        public override void Process(EventConnection ps) 
        {
            Post(ps);
        }

        public override void Handle(GameConnection conn, Player player)
        {
            if (player.RoomPlayer == null)
                return;

            if (Cmd == (byte)Commands.LEAVE)
                player.RoomPlayer.Room.RemovePlayer(player.PlayerId);
            else if (Cmd == (byte)Commands.KICK)
            {
                if (player.PlayerId != player.RoomPlayer.Room.MasterId)
                    return; // cannot kick master

                var target = player.RoomPlayer.Room.GetPlayer(PlayerId);
                if (target != null)
                    player.RoomPlayer.Room.RemovePlayer(target.Player.PlayerId);
            }

        }
    }
}
