using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
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
            { 0, new Spawn(){ X=48, Y=0, Z=48} }, // Garden
            { 1, new Spawn(){ X=48, Y=0, Z=48} }, // Diorama
            { 2, new Spawn(){ X=64, Y=0, Z=64} }, // Skycastle
            { 3, new Spawn(){ X=34, Y=0, Z=34} }, // Ossyria
            { 4, new Spawn(){ X=62, Y=0, Z=18} }, // Dollhouse
            { 5, new Spawn(){ X=68, Y=0, Z=38} }, // City
            { 6, new Spawn(){ X=58, Y=0, Z=60} }, // Bunker
            { 7, new Spawn(){ X=58, Y=0, Z=58} }, // Temple
            { 8, new Spawn(){ X=37, Y=0, Z=92} }, // Brdige
            { 9, new Spawn(){ X=66, Y=0, Z=66} } // Castaway
        };

        public void Initialize(RoomSession roomSession)
        {

        }

        public void Tick() { }

        public void SyncPlayer(RoomSessionPlayer player)
        {

        }
    }
}
