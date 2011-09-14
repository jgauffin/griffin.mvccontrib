using System;
using System.Data;

namespace Griffin.MvcContrib.Providers.Membership.SqlRepository
{
    /// <summary>
    /// Extension methods for different SQL classes
    /// </summary>
    public static class SqlExtensions
    {
        private static readonly DateTime SqlDate = new DateTime(1754, 1, 1).ToUniversalTime();

        /// <summary>
        /// convert a DateTime loaded from the database into a .NET datetime
        /// </summary>
        /// <param name="instance">Data record containing the field</param>
        /// <param name="name">Name of column</param>
        /// <returns>A proper date time</returns>
        /// <remarks>
        /// Used to convert null and SqlServers minvalue to DateTime.MinValue
        /// </remarks>
        public static DateTime FromSqlDate(this IDataRecord instance, string name)
        {
            var value = instance[name];
            if (value == DBNull.Value || value == null)
                return DateTime.MinValue;

            var date = (DateTime) value;
            return date == SqlDate ? DateTime.MinValue : date;
        }

        /// <summary>
        /// Add a parameter to a command
        /// </summary>
        /// <param name="command">command instance</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        public static void AddParameter(this IDbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Always store DateTime.MinValue as 1754-01-01 to add support for SqlServer.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>SqlServer safe date time</returns>
        public static DateTime SqlSafe(this DateTime instance)
        {
            return instance == DateTime.MinValue ? SqlDate : instance;
        }

        /// <summary>
        /// Create a command and assign it a SQL statement
        /// </summary>
        /// <param name="connnection">Connection creating the command</param>
        /// <param name="sql">SQL statement</param>
        /// <returns>Command</returns>
        public static IDbCommand CreateCommand(this IDbConnection connnection, string sql)
        {
            var cmd = connnection.CreateCommand();
            cmd.CommandText = sql;
            return cmd;
        }
    }
}