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
    public class GCCharm : GameNetEvent
    {
        private static NetClassRepInstance<GCCharm> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCCharm", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk01;
        public uint Unk02;

        public GCCharm() : base(GameNetId.GC_CHARM, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Unk01);
            bitStream.Write(Unk02);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
