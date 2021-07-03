using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Qpang.Room.Session.GameItem.Items;

namespace Qserver.GameServer.Qpang.Room.Session.GameItem
{
    public class GameItemManager
    {
        public enum Item : uint
        {
            AMMO_CLIP = 1191182337,
            RED_MEDKIT = 1191182338,
            EVENT_ITEM = 1191182344,
            GREEN_MEDKIT = 1191182350,
            SKILL_CARD = 1191182351,
            GOLD_COIN = 1191182352,
            SILVER_COIN = 1191182353,
            BRONZE_COIN = 1191182354
        };

        public static List<Item> PossibleItems = new List<Item> {
            Item.AMMO_CLIP,
            Item.RED_MEDKIT,
            Item.GREEN_MEDKIT,
            Item.SKILL_CARD,
        };

        public static Dictionary<Item, GameItem> MappedItems = new Dictionary<Item, GameItem>{
            { Item.RED_MEDKIT, new RedMedKit() },
            { Item.AMMO_CLIP, new AmmoClip() },
            { Item.GREEN_MEDKIT, new GreenMedKit() },
            { Item.SKILL_CARD, new SkillBook() },
            { Item.EVENT_ITEM, new EventItem() },
        };


        struct GameItemSpawn
        {
            public uint SpawnId;
            public Item Item;
            public DateTime LastPickUpTime;
            public Spawn spawn;
        };

        public static Dictionary<uint, Spawn> MapBounts = new Dictionary<uint, Spawn>()
        {
            { 0, Spawn(48, 0, 48) }, // Garden
            { 1, Spawn(48, 0, 48) }, // Diorama
            { 2, Spawn(64, 0, 64) }, // Skycastle
            { 3, Spawn(34, 0, 34) }, // Ossyria
            { 4, Spawn(62, 0, 18) }, // Dollhouse
            { 5, Spawn(68, 0, 38) }, // City
            { 6, Spawn(58, 0, 60) }, // Bunker
            { 7, Spawn(58, 0, 58) }, // Temple
            { 8, Spawn(37, 0, 92) }, // Brdige
            { 9, Spawn(66, 0, 66) } // Castaway
        };

        public void Initialize(RoomSession roomSession)
        {

        }

        public void tick() { }

        public void SyncPlayer(RoomSessionPlayer player)
        {

        }
    }
}
