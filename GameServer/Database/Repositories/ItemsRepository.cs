using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Database.Repositories
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

		public async Task<List<DBInventoryCard>> GetPlayerItems(uint playerId)
		{
			Task<IEnumerable<DBInventoryCard>> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				items = connection.QueryAsync<DBInventoryCard>("SELECT id, player_id, item_id, type, period_type, period, active, opened, giftable, boost_level, time FROM player_items WHERE player_id = @PlayerId", new { PlayerId = playerId }));
			return items.Result.ToList();
		}

		public async Task<ulong> PurchaseItem(InventoryCard card, Player player)
		{
			Task<ulong> items = null;
			await _sqlObjectFactory.GetConnection().UsingAsync(connection =>
				items = connection.QuerySingleAsync<ulong>("INSERT INTO player_items (player_id, item_id, period, period_type, type, active, opened, giftable, boosted, boost_level, time) " +
				"VALUES (@PlayerId, @ItemId, @Period, @PeriodType, @Type, @Active, @Opened, @Giftable, @Boosted, @BoostLevel, @Time); SELECT LAST_INSERT_ID()", new { PlayerId = player.PlayerId, ItemId = card.ItemId, Period = card.Period, PeriodType = card.PeriodeType, Type = card.Type, Active = card.IsActive, Opened = card.IsOpened, Giftable = card.IsGiftable, Boosted = (card.BoostLevel > 0), BoostLevel = card.BoostLevel, /* TODO */ Time=0}));
			return items.Result;
		}

	}
}
