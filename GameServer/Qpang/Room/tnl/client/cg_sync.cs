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
    public class CGSync : GameNetEvent
    {
        private static NetClassRepInstance<CGSync> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGSync", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Cmd;
        public uint PlayerId;
        public uint EntityId;
        public float PosX;
        public float PosY;
        public float PosZ;

        public CGSync() : base(GameNetId.CG_SYNC, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out Cmd);
            bitStream.Read(out PlayerId);
            bitStream.Read(out EntityId);
            bitStream.Read(out PosX);
            bitStream.Read(out PosY);
            bitStream.Read(out PosZ);
        }
        public override void Process(EventConnection ps) 
        {
            Post(ps);
        }
        public override void Handle(GameConnection conn, Player player)
        {
            var roomPlayer = player.RoomPlayer;
            if (roomPlayer == null)
                return;

            var session = roomPlayer.RoomSessionPlayer;
            if (session == null)
                return;

            // removed?
            //session.RoomSession.CreateEntity(session, EntityId)
        }
    }
}
