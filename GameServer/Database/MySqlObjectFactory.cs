using System;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace Qserver.GameServer.Database
{
	public class MySqlObjectFactory : IMySqlObjectFactory
	{
		private readonly string _connString;

		public MySqlObjectFactory(string connString)
		{
			_connString = connString;
			//_config = config;
		}

		public DbConnection GetConnection()
		{
			return new MySqlConnection(this._connString);
		}

		public DbCommand GetCommand()
		{
			return new MySqlCommand();
		}

		public DbCommand GetCommand(string sql)
		{
			return new MySqlCommand(sql);
		}

		public DbCommand GetCommand(string sql, DbConnection connection)
		{
			return new MySqlCommand(sql, (MySqlConnection)connection);
		}

		public DbParameter GetParameter(string parameterName, object value)
		{
			return new MySqlParameter(parameterName, value);
		}

        //DbConnection IMySqlObjectFactory.GetConnection()
        //{
        //    throw new NotImplementedException();
        //}

        //DbCommand IMySqlObjectFactory.GetCommand()
        //{
        //    throw new NotImplementedException();
        //}

        //DbCommand IMySqlObjectFactory.GetCommand(string sql)
        //{
        //    throw new NotImplementedException();
        //}

        //public DbCommand GetCommand(string sql, DbConnection connection)
        //{
        //    throw new NotImplementedException();
        //}

        DbParameter IMySqlObjectFactory.GetParameter(string parameterName, object value)
        {
            throw new NotImplementedException();
        }
    }
}
