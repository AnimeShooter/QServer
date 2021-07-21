using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNL.Entities;
using TNL.Utils;

namespace Qserver.GameServer.Qpang
{
    public class CGAuth : GameNetEvent
    {
        public uint playerId;
        public uint cmd;
        public byte unk03 = 3;
        public uint unk04;
        public ushort port;
        public uint ip;
        public uint unk07;
        public uint unk08;
        public uint unk09;
        public uint unk10;

        enum CMD : uint
        {
            REQUEST = 0,
            AUTHENTICATED,
            FAIL
        }

        public CGAuth() : base(GameNetId.CG_AUTH, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }
        public CGAuth(uint playerId, uint cmd) : base(GameNetId.CG_AUTH, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) 
        {
            this.playerId = playerId;
            this.cmd = cmd;
            this.unk03 = (byte)CMD.AUTHENTICATED;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(playerId);
            bitStream.Write(cmd);
            bitStream.Write(unk03);
            bitStream.Write(unk04);
            bitStream.Write(port);
            bitStream.Write(ip);
            bitStream.Write(unk07);
            bitStream.Write(unk08);
            bitStream.Write(unk09);
            bitStream.Write(unk10);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out playerId);
            bitStream.Read(out cmd);
            bitStream.Read(out unk03);
            bitStream.Read(out unk04);
            bitStream.Read(out port);
            bitStream.Read(out ip);
            bitStream.Read(out unk07);
            bitStream.Read(out unk08);
            bitStream.Read(out unk09);
            bitStream.Read(out unk10);
        }
        public override void Process(EventConnection ps) { }

        public override void Handle(GameConnection conn, Player player)
        {
            if (cmd != (byte)CMD.REQUEST)
                return;

            if (Game.Instance.RoomServer.CreateConnection(playerId, base.GameConnection))
                conn.PostNetEvent(new CGAuth(playerId, (uint)CMD.AUTHENTICATED));
            else
                conn.PostNetEvent(new CGAuth(playerId, (uint)CMD.FAIL));
        }
    }
}
