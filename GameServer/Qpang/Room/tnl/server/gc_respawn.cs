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
    public class GCRespawn : GameNetEvent
    {
        private static NetClassRepInstance<GCRespawn> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCRespawn", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint Cmd;
        public uint CharacterId;
        public float X;
        public float Y;
        public float Z;
        public byte IsVip;

        public GCRespawn() : base(GameNetId.GC_RESPAWN, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public GCRespawn(uint playerId, uint characterId, uint cmd, float x = 0xFF, float y = 0xFF, float z = 0xFF, byte isVip = 0) : base(GameNetId.GC_RESPAWN, GuaranteeType.Guaranteed, EventDirection.DirAny) 
        {
            PlayerId = playerId;
            Cmd = cmd;
            CharacterId = characterId;
            X = x;
            Y = y;
            Z = z;
            IsVip = isVip;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(PlayerId);
            bitStream.Write(Cmd);
            bitStream.Write(CharacterId);
            bitStream.Write(X);
            bitStream.Write(Y);
            bitStream.Write(Z);
            bitStream.Write(IsVip);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
