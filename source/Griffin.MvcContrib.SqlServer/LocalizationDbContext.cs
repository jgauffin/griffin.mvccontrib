using System;
using System.Data;
using System.Diagnostics;
using Griffin.MvcContrib.SqlServer.Localization;

namespace Griffin.MvcContrib.SqlServer
{
    /// <summary>
    /// Default implementation of the db context
    /// </summary>
    /// <remarks>Should be registered with a scoped lifetime</remarks>
    public class LocalizationDbContext : ILocalizationDbContext, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationDbContext"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        public LocalizationDbContext(AdoNetConnectionFactory connectionFactory)
        {
            if (connectionFactory == null) throw new ArgumentNullException("connectionFactory");
            Connection = connectionFactory.CreateConnection();
        }

        /// <summary>
        /// Gets open connection
        /// </summary>
        public IDbConnection Connection { get; private set; }


        /// <summary>
        /// Gets prefix used for query parameters (for instance '@' in Sql Server)
        /// </summary>
        public virtual char ParameterPrefix
        {
            get { return '@'; }
        }

        /// <summary>
        /// Change prefix to the one used in your db
        /// </summary>
        /// <param name="sql">Query which is using @@@ for parameters.</param>
        /// <returns>
        /// Query with valid parameters
        /// </returns>
        /// <example>
        ///   <code>
        /// public void ChangePrefix(string sql)
        /// {
        /// return sql.Replace("@@@", ParameterPrefix);
        /// }
        ///   </code>
        ///   </example>
        public virtual string ChangePrefix(string sql)
        {
            if (sql == null) throw new ArgumentNullException("sql");
            return ParameterPrefix == '@' ? sql : sql.Replace('@', ParameterPrefix);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Connection == null)
                return;

            Connection.Dispose();
            Connection = null;
        }
    }
}