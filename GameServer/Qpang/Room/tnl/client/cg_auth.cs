﻿using System;
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

        enum Command : uint
        {
            REQUEST = 0,
            AUTHENTICATED,
            FAIL
        }

        public CGAuth() : base(GameNetId.CG_AUTH, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }
        public CGAuth(uint playerId, uint cmd) : base(GameNetId.CG_AUTH, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) 
        {
            this.PlayerId = playerId;
            this.Cmd = cmd;
            this.unk03 = (byte)Command.AUTHENTICATED;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(PlayerId);
            bitStream.Write(Cmd);
            bitStream.Write((byte)unk03);
            bitStream.Write(unk04);
            bitStream.Write(Port);
            bitStream.Write(Ip);
            bitStream.Write(unk07);
            bitStream.Write(unk08);
            bitStream.Write(unk09);
            bitStream.Write(unk10);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out Cmd);
            bitStream.Read(out unk03);
            bitStream.Read(out unk04);
            bitStream.Read(out Port);
            bitStream.Read(out Ip);
            bitStream.Read(out unk07);
            bitStream.Read(out unk08);
            bitStream.Read(out unk09);
            bitStream.Read(out unk10);
        }
        public override void Process(EventConnection ps) { }

        public override void Handle(GameConnection conn, Player player)
        {
            if (Cmd != (byte)Command.REQUEST)
                return;

            if (Game.Instance.RoomServer.CreateConnection(PlayerId, base.GameConnection))
                conn.PostNetEvent(new CGAuth(PlayerId, (uint)Command.AUTHENTICATED));
            else
                conn.PostNetEvent(new CGAuth(PlayerId, (uint)Command.FAIL));
        }
    }
}