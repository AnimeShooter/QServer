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
    public class CCUserInfo : GameNetEvent
    {
        private static NetClassRepInstance<CCUserInfo> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CCUserInfo", NetClassMask.NetClassGroupGameMask, 0);
        }
        public CCUserInfo() : base(GameNetId.CC_USERINFO, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }

        public uint unk01 = 0;
        public uint unk02;
        public ushort unk03;
        public byte unk04;
        public ushort unk05;
        public byte unk06;
        public uint unk07;
        public uint unk08;
        public uint unk09;
        public uint unk10;
        public uint unk11;
        public uint unk12;
        public uint unk13;
        public uint unk14;
        public byte unk15;
        public uint unk16;
        public byte unk17;

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out unk01);
            bitStream.Read(out unk02);
            bitStream.Read(out unk03);
            bitStream.Read(out unk04);
            bitStream.Read(out unk05);
            bitStream.Read(out unk06);
            bitStream.Read(out unk07);
            bitStream.Read(out unk08);
            bitStream.Read(out unk09);
            bitStream.Read(out unk10);
            bitStream.Read(out unk11);
            bitStream.Read(out unk12);
            bitStream.Read(out unk13);
            bitStream.Read(out unk14);
            bitStream.Read(out unk15);
            bitStream.Read(out unk16);
        }
        public override void Process(EventConnection ps) { }
    }
}
