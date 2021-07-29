using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qserver.GameServer.Qpang;

namespace Qserver.Database.Repositories
{
	public struct DBItem
    {
		public uint seq_id;
		public uint item_id;
		public string name;
		public byte type;
		public ushort aid;
		public byte pay_type;
		public uint price;
		public byte use_up;
		public ushort period;
		public ushort level;
		public byte status;
		public uint sold_count;
		public uint stock;
		public uint order;
	}

	public struct DBWeapon
    {
		public uint id;
		public string code_name;
		public uint item_id;
		public ushort damage;
		public ushort clip_size;
		public byte clip_amount;
		public byte weight;
		public byte effect_id;
		public byte chance;
		public byte duration;
    }

	public struct DBInventoryCard
	{
		public ulong id;
		public uint player_id;
		public uint item_id;
		public byte type;
		public ushort period_type;
		public ushort period;
		public byte active;
		public byte opened;
		public byte giftable;
		public byte boost_level;
		public uint time; // uint?
	}

	public struct DBPlayerEquiped
	{
		public ulong id;
		public ulong player_id;
		public ushort character_id;
		public ulong melee;
		public ulong primary;
		public ulong secondary;
		public ulong Throw;
		public ulong head;
		public ulong face;
		public ulong body;
		public ulong hands;
		public ulong legs;
		public ulong shoes;
		public ulong back;
		public ulong side;	
	}

	public class ItemsRepository
    {
		public ItemsRepository(IMySqlObjectFactory sqlObjectFactory)
		{
			_sqlObjectFactory = sqlObjectFactory;
		}

		private readonly IMySqlObjectFactory _sqlObjectFactory;

		public async Task<List<DBItem>> GetItems()
		{
			Task<IEnumerable<DBItem>> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection => 
				items = connection.QueryAsync<DBItem>("SELECT seq_id, item_id, name, type, aid, pay_type, price, use_up, period, level, status, sold_count, stock, `order` FROM items"));
			return items.Result.ToList();
		}

		public async Task<List<DBWeapon>> GetWeapons()
		{
			Task<IEnumerable<DBWeapon>> weapons = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				weapons = connection.QueryAsync<DBWeapon>("SELECT damage, code_name, item_id, damage, clip_size, clip_amount, weight, effect_id, chance, duration FROM weapons"));
			return weapons.Result.ToList();
		}

		public async Task<List<DBInventoryCard>> GetInventoryCards(uint playerId)
		{
			Task<IEnumerable<DBInventoryCard>> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				items = connection.QueryAsync<DBInventoryCard>("SELECT id, player_id, item_id, type, period_type, period, active, opened, giftable, boost_level, time FROM player_items WHERE player_id = @PlayerId", new { PlayerId = playerId }));
			return items.Result.ToList();
		}

		public async Task<List<DBPlayerEquiped>> GetCharactersEquips(uint playerId)
		{
			Task<IEnumerable<DBPlayerEquiped>> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				items = connection.QueryAsync<DBPlayerEquiped>("SELECT id, player_id, character_id, melee, `primary`, secondary, `throw`, head, face, body, hands, legs, shoes, back, side FROM player_equipment WHERE player_id = @PlayerId", new { PlayerId = playerId }));
			return items.Result.ToList();
		}

		public async Task UpdateCharactersEquips(ulong[] equips, ushort characterId, ulong playerId)
		{
			Task<IEnumerable<DBPlayerEquiped>> test = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				test = connection.QueryAsync<DBPlayerEquiped>("UPDATE player_equipment SET " +
				"head = @Head, face = @Face, body = @Body, hands = @Hands, legs = @Legs, shoes = @Shoes, back = @Back, side = @Side, `primary` = @Primary, secondary = @Secondary, `throw` = @Throwy, melee = @Melee " +
				"WHERE player_id = @PlayerId AND character_id = @CharacterId",
				new { PlayerId = playerId, CharacterId = characterId, Head = equips[0], Face = equips[1], Body = equips[2], Hands = equips[3], Legs = equips[4], Shoes = equips[5], Back = equips[6], Side = equips[7], Primary = equips[9], Secondary = equips[10], Throwy = equips[11], Melee = equips[12] })) ;
			var test2 = test.Result;
			return;
		}

		public async Task<ulong> CreateItem(InventoryCard card, Player player)
		{
			Task<ulong> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				items = connection.QuerySingleAsync<ulong>("INSERT INTO player_items (player_id, item_id, period, period_type, type, active, opened, giftable, boosted, boost_level, time) " +
				"VALUES (@PlayerId, @ItemId, @Period, @PeriodType, @Type, @Active, @Opened, @Giftable, @Boosted, @BoostLevel, @Time); SELECT LAST_INSERT_ID()", new { PlayerId = player.PlayerId, ItemId = card.ItemId, Period = card.Period, PeriodType = card.PeriodeType, Type = card.Type, Active = card.IsActive, Opened = card.IsOpened, Giftable = card.IsGiftable, Boosted = (card.BoostLevel > 0), BoostLevel = card.BoostLevel, /* TODO */ Time=0}));
			return items.Result;
		}

	}
}
