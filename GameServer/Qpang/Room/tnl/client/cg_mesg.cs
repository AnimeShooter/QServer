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
    public class CGMesg : GameNetEvent
    {
        private static NetClassRepInstance<CGMesg> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGMesg", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint Cmd;
        public string Nickname;
        public string Message;

        public CGMesg() : base(GameNetId.CG_MESG, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out Cmd);
            var nickBuff = new ByteBuffer(16);
            bitStream.Read(nickBuff);
            Nickname = ByteBufferToString(nickBuff);
            var msgBuff = new ByteBuffer(254);
            bitStream.Read(msgBuff);
            Message = ByteBufferToString(msgBuff); // TODO: debug?
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

            Message = Game.Instance.ChatManager.Chat(player, Message);
            var room = roomPlayer.Room;
            var roomSession = room.RoomSession;

            // NOTE: check muted?

            if (roomPlayer.Spectating && Message != "")
            {
                player.Broadcast("You cannot chat while spectating.");
                return;
            }

            if (Message == "")
                return;

            if (roomSession == null)
                room.Broadcast<GCMesg>(player.PlayerId, Cmd, player.Name, Message);
            else
                if (roomPlayer.Playing)
                roomSession.Relay<GCMesg>(player.PlayerId, Cmd, player.Name, Message);
            else
                room.BroadcastWaiting<GCMesg>(player.PlayerId, Cmd, player.Name, Message);

        }
    }
}
