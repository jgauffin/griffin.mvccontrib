using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Griffin.MvcContrib.SqlServer.Tests
{
    /// <summary>
    /// Credits: http://blogs.infragistics.com/blogs/jess_chadwick/archive/2008/11/17/easier-automated-database-testing-with-sql-express.aspx
    /// </summary>
    public class SqlExpressTestDatabase : IDisposable
    {
        private readonly string _connectionString;
        private readonly string _databaseFilename;
        private bool _invoked = false;

        public string ConnectionString { get { return _connectionString; } }
        public string Schema { get; private set; }
        public IDbConnection Connection { get; set; }

        public SqlExpressTestDatabase(string schema)
        {
            Schema = schema;
            _databaseFilename = Path.Combine(Path.GetTempPath(), "Db" + Guid.NewGuid().ToString("N") + ".mdf");
            _connectionString = string.Format(
                @"Data Source=.\SQLExpress;AttachDbFilename={1};Initial Catalog={0}; Integrated Security=true;User Instance=True;",
                //@"Server=.\SQLEXPRESS; Integrated Security=true;Database={0};AttachDbFileName={1};Trusted_Connection=Yes",
                Path.GetFileNameWithoutExtension(_databaseFilename), _databaseFilename);

            CreateDatabase();
            Connection = new SqlConnection(_connectionString);
            Connection.Open();
            ExecuteQuery(Schema);
        }

        public void Dispose()
        {
            Connection.Dispose();
            Connection = null;
            GC.Collect();
            SqlConnection.ClearAllPools();
            try
            {
                File.Delete(_databaseFilename);
           }
            catch(Exception)
            {
                // Delete file at reboot.
                NativeMethods.MoveFileEx(_databaseFilename, null, MoveFileFlags.DelayUntilReboot);
            }
        }
        
        // Create a new file-based SQLEXPRESS database
        // (Credit to Louis DeJardin - thanks! http://snurl.com/5nbrc)
        protected void CreateDatabase()
        {
            if (_invoked)
                throw new InvalidOperationException("Already invoked");

            _invoked = true;
            var databaseName = Path.GetFileNameWithoutExtension(_databaseFilename);

            using (var connection = new SqlConnection(
                "Data Source=.\\sqlexpress;Initial Catalog=tempdb;" +
                "Integrated Security=true;User Instance=True;"))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        /*"CREATE DATABASE " + databaseName +
                        " ON PRIMARY (NAME=" + databaseName +
                        ", FILENAME='" + _databaseFilename + "')";*/
                        string.Format(
                            "CREATE DATABASE {0} ON PRIMARY (NAME={0}, FILENAME='{1}', SIZE = 10000KB) LOG ON ( NAME = N'{0}_Log', FILENAME = N'{1}_Log.ldf', SIZE = 512KB)",
                            databaseName, _databaseFilename);
                    command.ExecuteNonQuery();

                    //connection.ChangeDatabase(databaseName);
                   // command.CommandText=
                   

                    command.CommandText =
                        "EXEC sp_detach_db '" + databaseName + "', 'true'";
                    command.ExecuteNonQuery();
                }
            }

        }

        public void ExecuteQuery(string query)
        {
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
