using Griffin.MvcContrib.SqlServer.Localization;

namespace Griffin.MvcContrib.SqlServer.Tests
{
    public class SqlExpressConnectionFactory : SqlExpressTestDatabase, ILocalizationDbContext
    {
        public SqlExpressConnectionFactory(string schema) : base(schema)
        {
        }

        /// <summary>
        /// Gets prefix used for query parameters (for instance '@' in Sql Server)
        /// </summary>
        public char ParameterPrefix
        {
            get { return '@'; }
        }

        /// <summary>
        /// Change prefix to the one used in your db
        /// </summary>
        /// <param name="sql">Query which is using @@@ for parameters.</param>
        /// <returns>Query with valid parameters</returns>
        /// <example>
        /// <code>
        /// public void ChangePrefix(string sql)
        /// {
        ///     return sql.Replace("@@@", ParameterPrefix);
        /// }
        /// </code>
        /// </example>
        public string ChangePrefix(string sql)
        {
            return sql.Replace("@@@", ParameterPrefix.ToString());
        }
    }
}