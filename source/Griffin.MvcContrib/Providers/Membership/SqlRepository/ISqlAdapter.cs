namespace Griffin.MvcContrib.Providers.Membership.SqlRepository
{
    /// <summary>
    /// Used to generate vendor specific SQL statements.
    /// </summary>
    public interface ISqlAdapter
    {
        /// <summary>
        /// Add paging to the SQL
        /// </summary>
        /// <param name="sql">SQL statement that needs paging.</param>
        /// <param name="page">Onebased index.</param>
        /// <param name="rowsPerPage">The number of rows per page.</param>
        /// <returns>Paged SQL statement</returns>
        string PageSql(string sql, int page, int rowsPerPage);
    }
}