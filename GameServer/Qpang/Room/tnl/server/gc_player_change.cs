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
    public class GCPlayerChange : GameNetEvent
    {
        private static NetClassRepInstance<GCPlayerChange> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPlayerChange", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public byte Cmd;
        public uint Value;
        public uint[] Equipment = new uint[9];

        public GCPlayerChange() : base(GameNetId.GC_PLAYER_CHANGE, GuaranteeType.Guaranteed, EventDirection.DirAny) { }
        public GCPlayerChange(Player player, byte cmd, uint value) : base(GameNetId.GC_PLAYER_CHANGE, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            PlayerId = player.PlayerId;
            Cmd = cmd;
            Value = value;
            Equipment = player.EquipmentManager.GetArmorItemIdsByCharacter(player.Character);
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(PlayerId);
            bitStream.Write(Cmd);
            bitStream.Write(Value);

            for (int i = 0; i < 9; i++)
                bitStream.Write(Equipment[i]);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
