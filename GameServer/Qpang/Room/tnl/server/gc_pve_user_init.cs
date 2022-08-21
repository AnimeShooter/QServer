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
    public class GCPvEUserInit : GameNetEvent
    {
        private static NetClassRepInstance<GCPvEUserInit> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCPvEUserInit", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint State; // 88 (0x20, 0x40, 0x80 TODO find flag usage?)
        public uint PlayerId; // 92
        public ushort CharacterId; // 130
        public uint SelectedWeapon; // 168
        public ushort WeaponCount; // 172 

        public uint[] Armor = new uint[9];
        public uint[] Weapons = new uint[4];

        public string Nickname; // 252

        public byte PlayerRank; // 240 unk 10, unk11// aka prestiege
        public uint Coins; // 244; actionId, level, 
        public ushort DoubleCoins; // 248; key, hp, refers

        public GCPvEUserInit() : base(GameNetId.GC_PVE_USER_INIT, GuaranteeType.Guaranteed, EventDirection.DirAny) { }

        public GCPvEUserInit(RoomSessionPlayer sessionPlayer, bool spectatorMode = false) : base(GameNetId.GC_PVE_USER_INIT, GuaranteeType.Guaranteed, EventDirection.DirServerToClient)
        {
            Player player = sessionPlayer.Player;

            Nickname = player.Name;
            State = 0x20;
            PlayerId = player.PlayerId;
            CharacterId = player.Character;
            SelectedWeapon = player.EquipmentManager.GetDefaultWeapon();

            WeaponCount = (ushort)player.EquipmentManager.GetWeaponsByCharacter(CharacterId).Length;
            Weapons = player.EquipmentManager.GetWeaponItemIdsByCharacter(CharacterId);
            Armor = player.EquipmentManager.GetArmorItemIdsByCharacter(CharacterId);
            PlayerRank = spectatorMode ? (byte)3 : player.Rank;
            Coins = player.Coins;

            //Refers = player.Prestige;
            //DoubleCoins = (ushort)(player.EquipmentManager.GetBaseHealth() + player.EquipmentManager.GetBonusHealth());
            //Experience = player.Experience;
        }

        public GCPvEUserInit(RoomPlayer roomPlayer, bool spectatorMode = false) : base(GameNetId.GC_PVE_USER_INIT, GuaranteeType.Guaranteed, EventDirection.DirServerToClient)
        {
            Player player = roomPlayer.Player;

            Nickname = player.Name;
            State = 0x20;
            PlayerId = player.PlayerId;
            CharacterId = player.Character;
            SelectedWeapon = player.EquipmentManager.GetDefaultWeapon();

            WeaponCount = (ushort)player.EquipmentManager.GetWeaponsByCharacter(CharacterId).Length; // always 4?
            Weapons = player.EquipmentManager.GetWeaponItemIdsByCharacter(CharacterId);
            Armor = player.EquipmentManager.GetArmorItemIdsByCharacter(CharacterId);
            PlayerRank = spectatorMode ? (byte)3 : player.Rank;
            //Refers = player.Prestige;
            //DoubleCoins = (ushort)(player.EquipmentManager.GetBaseHealth() + player.EquipmentManager.GetBonusHealth());
            //Experience = player.Experience;

            Coins = player.Coins;

            DoubleCoins = 0; // Refers? (!= 0 == double)

            // rank, exp, key?
        }


        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(State);
            bitStream.Write(PlayerId);
            bitStream.Write(CharacterId);
            bitStream.Write(SelectedWeapon);
            bitStream.Write(WeaponCount);

            foreach (uint item in Armor)
                bitStream.Write(item);

            foreach (uint item in Weapons)
                bitStream.Write(item);

            WriteWString(bitStream, Nickname, 16);

            bitStream.Write(PlayerRank);
            bitStream.Write(Coins);
            bitStream.Write(DoubleCoins);

        }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
