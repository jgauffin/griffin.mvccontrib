using System;
using System.Data;
using System.Data.SqlClient;
using Griffin.MvcContrib.SqlServer.Localization;

namespace Griffin.MvcContrib.SqlServer.Tests
{
	public class ConnectionFactory : ILocalizationDbContext 
	{
		private SqlConnection _connection;

		public ConnectionFactory()
		{
			_connection = new SqlConnection(string.Format(@"Server=.\SQLExpress;Integrated Security=True;Database=MvcContrib;",
				AppDomain.CurrentDomain.BaseDirectory)); //.Replace("bin\\", "").Replace("Debug", "")
			_connection.Open();
		}

		public IDbConnection Connection
		{
			get { return _connection; }
		}

		public char ParameterPrefix
		{
			get { return '@'; }
		}

		public string ChangePrefix(string sql)
		{
			return sql;
		}
	}
}