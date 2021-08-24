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
    public class CGMotion : GameNetEvent
    {
        private static NetClassRepInstance<CGMotion> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGMotion", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Cmd;
        public uint unk02;
        public uint unk03;
        public uint unk04;
        public uint unk05;
        public uint unk06;
        public uint unk07;
        public uint unk08;
        public uint unk09;
        public uint PlayerId;

        public CGMotion() : base(GameNetId.CG_MOTION, GuaranteeType.Unguaranteed, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out Cmd);
            bitStream.Read(out unk02);
            bitStream.Read(out unk03);
            bitStream.Read(out unk04);
            bitStream.Read(out unk05);
            bitStream.Read(out unk06);
            bitStream.Read(out unk07);
            bitStream.Read(out unk08);
            bitStream.Read(out unk09);
            bitStream.Read(out PlayerId);
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

            Console.WriteLine($"MOTION-[{DateTime.UtcNow.ToString()}][{player.Name}] {Cmd.ToString("X8")} {unk02} {unk03} {unk04} | {unk05} {unk07} {unk07} {unk08} {unk09} {PlayerId}");

            roomSession.RelayPlaying<GCMotion>(Cmd, unk02, unk03, unk04, unk05, unk06, unk07, unk08, unk09, PlayerId, (byte)0);
        }
    }
}
