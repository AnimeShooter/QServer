﻿using System;
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
    public class GGReload : GameNetEvent
    {
        private static NetClassRepInstance<GGReload> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GGReload", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk01;
        public byte Unk02;

        public GGReload() : base(GameNetId.GG_RELOAD, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(Unk01);
            bitStream.Write(Unk02);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
