using System;
using System.Data;
using System.Data.SqlClient;

namespace Griffin.MvcContrib.SqlServer.Localization
{
    public class LocalizationConnectionFactory : ILocalizationDbContext
    {
        #region ILocalizationDbContext Members

        public IDbConnection Connection
        {
            get { throw new NotImplementedException(); }
        }

        public char ParameterPrefix
        {
            get { return '@'; }
        }

        public string ChangePrefix(string sql)
        {
            return ParameterPrefix == '@' ? sql : sql.Replace('@', ParameterPrefix);
        }

        #endregion

        public IDbConnection CreateConnection()
        {
            return new SqlConnection();
        }
    }
}