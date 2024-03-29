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
    public class GCPvEDoor : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEDoor> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEDoor", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Uid; // 88
        public bool Triggerd; //

        public GCPvEDoor() : base(GameNetId.GC_PVE_DOOR, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCPvEDoor(uint uid, bool triggerd) : base(GameNetId.GC_PVE_DOOR, GuaranteeType.Guaranteed, EventDirection.DirAny) 
        {
            Uid = uid;
            Triggerd = triggerd;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(Uid);
            bitStream.Write(Triggerd);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Uid);
            bitStream.Read(out Triggerd);
        }
        public override void Process(EventConnection ps) { }
    }
}
