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
    public class GCMesg : GameNetEvent
    {
        private static NetClassRepInstance<GCMesg> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCMesg", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint Cmd;
        public string Nickname;
        public string Message;

        public GCMesg() : base(GameNetId.GC_MESG, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public GCMesg(uint playerId, uint cmd, string nickname, string message) : base(GameNetId.GC_MESG, GuaranteeType.Guaranteed, EventDirection.DirAny) 
        {
            PlayerId = playerId;
            Cmd = cmd;
            Nickname = nickname;
            Message = message;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(PlayerId);
            bitStream.Write(Cmd);
            WriteWString(bitStream, Nickname, 16);
            WriteWString(bitStream, Message, (byte)(Message.Length % 254));
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
