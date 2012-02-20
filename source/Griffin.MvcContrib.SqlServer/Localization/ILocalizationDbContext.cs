using System.Data;

namespace Griffin.MvcContrib.SqlServer.Localization
{
    /// <summary>
    /// Used to provide a connection for the repositories
    /// </summary>
    public interface ILocalizationDbContext
    {
        /// <summary>
        /// Gets an open and valid connection
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// Gets prefix used for query parameters (for instance '@' in Sql Server)
        /// </summary>
        char ParameterPrefix { get; }

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
        string ChangePrefix(string sql);
    }
}