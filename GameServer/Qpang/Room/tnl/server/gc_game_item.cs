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
    public class GCGameItem : GameNetEvent
    {
        private static NetClassRepInstance<GCGameItem> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCGameItem", NetClassMask.NetClassGroupGameMask, 0);
        }

        public struct Item
        {
            public uint ItemId;
            public uint ItemUid;
            public float X;
            public float Y;
            public float Z;
            public uint SkillId;
        }

        public uint PlayerId;
        public byte Cmd = 6;
        public uint unk03 = 0;
        public uint unk04 = 0;
        public uint ItemId;
        public uint Uid;
        public uint SkillId;
        public uint unk08 = 0;
        public ushort count;
        public uint WeaponId;

        public List<Item> Items;

        public GCGameItem() : base(GameNetId.GC_GAME_ITEM, GuaranteeType.Guaranteed, EventDirection.DirServerToClient) { }

        public GCGameItem(byte cmd, List<Item> items, uint weaponId = 0x00) : base(GameNetId.GC_GAME_ITEM, GuaranteeType.Guaranteed, EventDirection.DirServerToClient) 
        {
            Cmd = cmd;
            this.Items = items;
            WeaponId = weaponId;
        }

        public GCGameItem(byte cmd, uint playerId, uint itemId, uint uid, uint skillId = 0x00) : base(GameNetId.GC_GAME_ITEM, GuaranteeType.Guaranteed, EventDirection.DirServerToClient)
        {
            Cmd = cmd;
            PlayerId = playerId;
            ItemId = itemId;
            Uid = uid;
            SkillId = skillId;
        }

        public override void Pack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Write(PlayerId);
            bitStream.Write(Cmd);
            bitStream.Write(unk03);
            bitStream.Write(unk04);
            bitStream.Write(ItemId);
            bitStream.Write(Uid);
            bitStream.Write(SkillId);
            bitStream.Write(unk08);
            bitStream.Write((ushort)Items.Count);

            foreach(var item in Items)
            {
                bitStream.Write((uint)0);
                bitStream.Write(item.ItemId);
                bitStream.Write(item.ItemUid);
                bitStream.Write(item.SkillId);
                bitStream.Write((uint)0);
                bitStream.Write(item.X);
                bitStream.Write(item.Y);
                bitStream.Write(item.X);
                bitStream.Write(WeaponId);
            }
        }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
