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
    public class CGPvEHitObject : GameNetEvent
    {
        private static NetClassRepInstance<CGPvEHitObject> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGPvEHitObject", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk01;
        public uint Unk02;
        public uint Unk03;
        public byte Cmd;

        public CGPvEHitObject() : base(GameNetId.CG_PVE_HIT_OBJECT, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out Unk01);
            bitStream.Read(out Unk02);
            bitStream.Read(out Unk03);
            bitStream.Read(out Cmd);
        }
        public override void Process(EventConnection ps) { }
    }
}
