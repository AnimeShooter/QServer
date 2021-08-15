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
    public class GCPvEMoveNodesNpc : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEMoveNodesNpc> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEMoveNodesNpc", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Unk1;
        public uint Unk2;

        public GCPvEMoveNodesNpc() : base(GameNetId.GC_PVE_MOVE_NODES_NPC, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(Unk1);
            bitStream.Write(Unk2);
            // TOOD: loop?
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Unk1);
            bitStream.Read(out Unk2);
            // TODO loop 
        }
        public override void Process(EventConnection ps) 
        {
            Post(ps);
        }
    }
}
