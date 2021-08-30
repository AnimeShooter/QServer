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
    public class GCPvENpcInit : GameNetEvent
    {
        private static NetClassRepInstance<GCPvENpcInit> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvENpcInit", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint Id; // 88
        public uint Uid; // 92
        public float X; // 96
        public float Y; // 100
        public float Z; // 104
        public ushort Unk6; // 108 // Type?
        public byte Unk7; // byte? 110 
        public uint Unk8; // 112

        public GCPvENpcInit() : base(GameNetId.GC_PVE_NPC_INIT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCPvENpcInit(uint id, uint uid, ushort type, byte subType, Position spawn) : base(GameNetId.GC_PVE_NPC_INIT, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            /*
                1: normal
                2: Throw rabbit
                3:
                4:
                5: 
                6: Laser
                7:
                8:
                9: flowerpot
                10: camera
                11: horse melee
                12: Boss
                13: boss?
                14: normal?
                15: white rabbit?
                16: throw pink
                17: throw white
                18: double ggun
                19: white double gunn
                20: Orange laser
                21: white laser
                22: throw mouse cat
                23: throw white cat
                24: gun orange
                25: gun white
                26: big Melee rat
            */
            Id = id; // type? // 1
            Uid = uid; // subtype? // 1, 2, 3, 6 (training rabbits?)
            X = spawn.X;
            Y = spawn.Y;
            Z = spawn.Z;
            Unk6 = type; // unk
            Unk7 = subType;  // unk
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(Id);
            bitStream.Write(Uid);
            bitStream.Write(X);
            bitStream.Write(Y);
            bitStream.Write(Z);
            bitStream.Write(Unk6);
            bitStream.Write(Unk7);
            bitStream.Write(Unk8);
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out Id);
            bitStream.Read(out Uid);
            bitStream.Read(out X);
            bitStream.Read(out Y);
            bitStream.Read(out Z);
            bitStream.Read(out Unk6);
            bitStream.Read(out Unk7);
            bitStream.Read(out Unk8);
        }
        public override void Process(EventConnection ps) { }
    }
}
