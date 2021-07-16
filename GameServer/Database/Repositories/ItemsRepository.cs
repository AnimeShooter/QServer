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

	}
}
