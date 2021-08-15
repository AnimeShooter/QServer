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
    public class GCPvEItemInit : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEItemInit> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEItemInit", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk1; // 92;
        public uint Unk2; // 96;
        public uint Unk3; // 100;
        public uint Unk4; // 104;

        public GCPvEItemInit() : base(GameNetId.GC_PVE_ITEM_INIT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Unk1);
            bitStream.Write(Unk2);
            bitStream.Write(Unk3);
            bitStream.Write(Unk4);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Unk1);
            bitStream.Read(out Unk2);
            bitStream.Read(out Unk3);
            bitStream.Read(out Unk4);
        }
        public override void Process(EventConnection ps) { }
    }
}
