using System.Data.Common;

namespace Qserver.Database
{
	public interface IMySqlObjectFactory
	{
		DbConnection GetConnection();
		DbCommand GetCommand();
		DbCommand GetCommand(string sql);
		DbCommand GetCommand(string sql, DbConnection connection);
		DbParameter GetParameter(string parameterName, object value);
	}
}
