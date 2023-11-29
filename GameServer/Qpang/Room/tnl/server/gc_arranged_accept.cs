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
    public class GCArrangedAccept : GameNetEvent
    {
        private static NetClassRepInstance<GCArrangedAccept> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCArrangedAccept", NetClassMask.NetClassGroupGameMask, 0);
        }

        //public const string Key = "123456";
        public uint SenderId;
        public uint TargetId;
        
        public uint Unk3 = 1; // 5; // 96
        public uint Count = 0; // 100
        public uint[] PossIP; // 108 + 8 * i
        public ushort[] PossPort; // 108 + 8 * i + 4
        public string Key = "12345678901234";

        public GCArrangedAccept() : base(GameNetId.GC_ARRANGED_ACCEPT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public GCArrangedAccept(uint senderId, uint targetId) : base(GameNetId.GC_ARRANGED_ACCEPT, GuaranteeType.Guaranteed, EventDirection.DirAny) 
        {
            SenderId = senderId;
            TargetId = targetId;
        }

        public GCArrangedAccept(uint senderId, uint targetId, string key) : base(GameNetId.GC_ARRANGED_ACCEPT, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            SenderId = senderId;
            TargetId = targetId;
            Key = key;
        }

        public GCArrangedAccept(uint senderId, uint targetId, string key, uint ip, ushort port) : base(GameNetId.GC_ARRANGED_ACCEPT, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            SenderId = senderId;
            TargetId = targetId;
            Key = key;
            PossIP = new uint[1] { ip };
            PossPort = new ushort[1] { port };
        }


        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(SenderId);
            bitStream.Write(TargetId);
            bitStream.Write(0xfff);

            if(PossIP == null)
            {
                bitStream.Write(1);
                bitStream.Write((uint)222);
                bitStream.Write((ushort)TargetId);
                WriteWString(bitStream, Key, (uint)Key.Length);
            }
            else
            {
                bitStream.Write(PossIP.Length); //bitStream.Write(1);
                //bitStream.Write((uint)222);
                //bitStream.Write((ushort)TargetId);

                for (int i = 0; i < PossIP.Length; i++)
                {
                    bitStream.Write(PossIP[i]); // possible DWORD IP
                    bitStream.Write(PossPort[i]); // possible PORT
                }
                WriteWString(bitStream, Key, (uint)Key.Length);
            }
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
