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
    public class CGAuth : GameNetEvent
    {
        private static NetClassRepInstance<CGAuth> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGAuth", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint Cmd;
        public byte unk03 = 3;
        public uint unk04;
        public ushort Port;
        public uint Ip;
        public uint unk07;
        public uint unk08;
        public uint unk09;
        public uint unk10;

        enum Commands : uint
        {
            REQUEST = 0,
            AUTHENTICATED,
            FAIL
        }

        public CGAuth() : base(GameNetId.CG_AUTH, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }
        public CGAuth(uint playerId, uint cmd) : base(GameNetId.CG_AUTH, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            this.PlayerId = playerId;
            this.Cmd = cmd;
            this.unk03 = (byte)Commands.AUTHENTICATED;
        }
        public CGAuth(uint playerId, uint cmd, uint ip, ushort port, uint unk07, uint unk08, uint unk09, uint unk10) : base(GameNetId.CG_AUTH, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            this.PlayerId = playerId;
            this.Cmd = cmd;
            this.unk03 = (byte)Commands.AUTHENTICATED;
            this.Ip = ip;
            this.Port = port;
            this.unk07 = unk07;
            this.unk08 = unk08;
            this.unk09 = unk09;
            this.unk10 = unk10;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(PlayerId);
            bitStream.Write(Cmd);
            bitStream.Write(unk03); // 1 - type
            bitStream.Write(unk04); // 0
            bitStream.Write(Port);  // 0??
            bitStream.Write(Ip);    // 0??
            bitStream.Write(unk07); // KEY?
            bitStream.Write(unk08);
            bitStream.Write(unk09);
            bitStream.Write(unk10);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out Cmd);
            bitStream.Read(out unk03);
            bitStream.Read(out unk04); // poss HWID related?
            bitStream.Read(out Port);
            bitStream.Read(out Ip); // obtain client IP:PORT
            bitStream.Read(out unk07); // 0x33393232 2293
            bitStream.Read(out unk08); // 0x2d353138 815-
            bitStream.Read(out unk09); // 0x4454464B KFTD - 352D5955 2D353138 33393232 0100007F (KFTDUY-5815-2293)
            bitStream.Read(out unk10); // 0x352d5955 UY-5

            string test = Encoding.ASCII.GetString(BitConverter.GetBytes(unk09)) +
                Encoding.ASCII.GetString(BitConverter.GetBytes(unk10)) +
                Encoding.ASCII.GetString(BitConverter.GetBytes(unk08)) +
                Encoding.ASCII.GetString(BitConverter.GetBytes(unk07));
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }

        public override void Handle(GameConnection conn, Player player)
        {
            if (Cmd != (byte)Commands.REQUEST)
                return;

            if (Game.Instance.RoomServer.CreateConnection(PlayerId, base.GameConnection, Ip, Port))
                conn.PostNetEvent(new CGAuth(PlayerId, (uint)Commands.AUTHENTICATED));
            else
                conn.PostNetEvent(new CGAuth(PlayerId, (uint)Commands.FAIL));
        }
    }
}
