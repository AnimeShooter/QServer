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
    public class GCJoin : GameNetEvent
    {
        private static NetClassRepInstance<GCJoin> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCJoin", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint State = 0x20;
        public uint PlayerId = 1;
        public ushort CharacterId = 333;
        public uint SelectedWeapon = 0;
        public uint Unk05 = 0;
        public byte Team = 1;
        public byte Ready = 0;
        public ushort WeaponCount = 4;

        public uint[] Armor = new uint[9];
        public uint[] Weapons = new uint[4];
        public string Nickname;

        public byte Unk10 = 0;
        public byte Unk11 = 0;

        public uint Level = 1;
        public uint ActionId = 0;
        public ushort Refers = 0;
        public ushort Life = 0;
        public byte PlayerRank = 1;
        public uint Experience = 0;
        public ushort PartnerKey = 0;


        public GCJoin() : base(GameNetId.GC_JOIN, GuaranteeType.GuaranteedOrdered, EventDirection.DirServerToClient) { }
        public GCJoin(RoomPlayer roomPlayer, bool spectatorMode = false) : base(GameNetId.GC_JOIN, GuaranteeType.Guaranteed, EventDirection.DirServerToClient) 
        {
            Player player = roomPlayer.Player;

            Nickname = player.Name;
            State = 0x20;
            PlayerId = player.PlayerId;
            CharacterId = player.Character;
            SelectedWeapon = player.EquipmentManager.GetDefaultWeapon();

            Team = roomPlayer.Team;
            Ready = roomPlayer.Ready ? (byte)1 : (byte)0;
            WeaponCount = (ushort)player.EquipmentManager.GetWeaponsByCharacter(CharacterId).Length;
            Weapons = player.EquipmentManager.GetWeaponItemIdsByCharacter(CharacterId);
            Armor = player.EquipmentManager.GetArmorItemIdsByCharacter(CharacterId);
            PlayerRank = spectatorMode ? (byte)3 : player.Rank;
            Refers = player.Prestige;
            Level = player.Level;
            Life = (ushort)(player.EquipmentManager.GetBaseHealth() + player.EquipmentManager.GetBonusHealth());
            Experience = player.Experience;
        }

        public GCJoin(RoomSessionPlayer session, bool spectatorMode = false) : base(GameNetId.GC_JOIN, GuaranteeType.Guaranteed, EventDirection.DirServerToClient)
        {
            Player player = session.Player;

            Nickname = player.Name;
            State = 0x20;
            PlayerId = player.PlayerId;
            CharacterId = player.Character;
            SelectedWeapon = player.EquipmentManager.GetDefaultWeapon();

            Team = session.Team;
            Ready = (byte)1;
            WeaponCount = (ushort)player.EquipmentManager.GetWeaponsByCharacter(CharacterId).Length;
            Weapons = player.EquipmentManager.GetWeaponItemIdsByCharacter(CharacterId);
            Armor = player.EquipmentManager.GetArmorItemIdsByCharacter(CharacterId);
            PlayerRank = spectatorMode ? (byte)3 : player.Rank;
            Refers = player.Prestige;
            Level = player.Level;
            Life = (ushort)(player.EquipmentManager.GetBaseHealth() + player.EquipmentManager.GetBonusHealth());
            Experience = player.Experience;
        }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {

        }
        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(State);
            bitStream.Write(PlayerId);
            bitStream.Write(CharacterId);
            bitStream.Write(SelectedWeapon);
            bitStream.Write(Unk05);
            bitStream.Write(Team);
            bitStream.Write(Ready);
            bitStream.Write(WeaponCount);
            
            foreach(uint item in Armor)
                bitStream.Write(item);
            
            foreach (uint item in Weapons)
                bitStream.Write(item);

            bitStream.Write((ushort)0); // NoName
            //WriteWStringMax(bitStream, Nickname, 16);

            bitStream.Write(Unk10);
            bitStream.Write(Unk11);
            bitStream.Write(Level);
            bitStream.Write((uint)(Level >> 2));
            bitStream.Write(ActionId);
            bitStream.Write(Refers);
            bitStream.Write(Life);
            bitStream.Write(PlayerRank);
            bitStream.Write(Experience);
            bitStream.Write(PartnerKey);
        }
        public override void Process(EventConnection ps) { }
    }
}
