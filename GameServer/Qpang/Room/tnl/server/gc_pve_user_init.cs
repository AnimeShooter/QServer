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
    public class GCPvEUserInit : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEUserInit> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEUserInit", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk1; // 88
        public uint Unk2; // 92
        public uint Unk3; // 130
        public uint Unk4; // 168
        public uint Unk5; // 172
        // loop
        // loop2
        public string Name; // 252

        // TODO

        public byte Unk6; // 240
        public uint Unk7; // 244;
        public ushort Unk8; // 248;

        public GCPvEUserInit() : base(GameNetId.GC_PVE_USER_INIT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            // TODO
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            // TODO
        }
        public override void Process(EventConnection ps) { }
    }
}
