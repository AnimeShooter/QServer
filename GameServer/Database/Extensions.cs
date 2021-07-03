using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Database
{
	public static class Extensions
	{
		public static DbCommand Command(this DbConnection connection, IMySqlObjectFactory sqlObjectFactory, string sql)
		{
			var command = sqlObjectFactory.GetCommand(sql, connection);
			return command;
		}

		public static DbCommand AddParameter(this DbCommand command, IMySqlObjectFactory sqlObjectFactory, string parameterName, object value)
		{
			var parameter = sqlObjectFactory.GetParameter(parameterName, value);
			command.Parameters.Add(parameter);
			return command;
		}

		public static void Using(this DbConnection connection, Action<DbConnection> action)
		{
			using (connection)
			{
				try
				{
					connection.Open();
					action(connection);
				}
				finally
				{
					connection.Close();
					connection.Dispose();
				}
			}
		}

		public static async Task UsingAsync(this DbConnection connection, Func<DbConnection, Task> action)
		{
			using (connection)
			{
				try
				{
					await connection.OpenAsync();
					await action(connection);
				}
				finally
				{
					connection.Close();
					connection.Dispose();
				}
			}
		}

		public static object GetObjectOrDbNull(this object value)
		{
			return value ?? DBNull.Value;
		}

		public static int? NullIntDbHelper(this DbDataReader reader, int index)
		{
			if (reader.IsDBNull(index)) return null;
			return reader.GetInt32(index);
		}

		public static string NullStringDbHelper(this DbDataReader reader, int index)
		{
			if (reader.IsDBNull(index)) return null;
			return reader.GetString(index);
		}

		public static Guid? NullGuidDbHelper(this IDataReader reader, int index)
		{
			if (reader.IsDBNull(index)) return null;
			return reader.GetGuid(index);
		}

		public static string NullToEmpty(this string text)
		{
			if (String.IsNullOrEmpty(text))
				return String.Empty;
			return text;
		}
	}
}
