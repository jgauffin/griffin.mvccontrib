using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Griffin.MvcContrib.SqlServer
{
    /// <summary>
    /// Creates database connections by loading the correct driver using app/web.config
    /// </summary>
    /// <remarks>Should be registered as a singleton</remarks>
    public class AdoNetConnectionFactory
    {
        private readonly string _connectionStringName;
        private readonly DbProviderFactory _factory;
        private ConnectionStringSettings _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string in app/web.config.</param>
        public AdoNetConnectionFactory(string connectionStringName)
        {
            _connectionStringName = connectionStringName;
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (_connectionString == null)
                throw new ArgumentOutOfRangeException("connectionStringName", string.Format("Failed to find connection string named '{0}'.", connectionStringName));

            _factory = DbProviderFactories.GetFactory(_connectionString.ProviderName);
        }

        
        /// <summary>
        /// Creates and open a connection.
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnection CreateConnection()
        {
            var connection = _factory.CreateConnection();
            if (connection == null)
                throw new InvalidOperationException(string.Format("Failed to build a ADO.NET connection using the connection string named '{0}'.", _connectionStringName));
            Trace.WriteLine("** Creating connection " + connection.GetHashCode());
            connection.ConnectionString = _connectionString.ConnectionString;
            connection.Open();
            return connection;
        }
    }
}