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
    public class CGMoveReport : GameNetEvent
    {
        private static NetClassRepInstance<CGMoveReport> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGMoveReport", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public float PosX;
        public float PosY;
        public float PosZ;
        public float unk05; // possible pitch
        public float unk06; // possible yaw (NOTE: used to anti cheat detection)


        public CGMoveReport() : base(GameNetId.CG_MOVE_REPORT, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out PosX);
            bitStream.Read(out PosY);
            bitStream.Read(out PosZ);
            bitStream.Read(out unk05);
            bitStream.Read(out unk06);
        }
        public override void Process(EventConnection ps) { }
    }
}
