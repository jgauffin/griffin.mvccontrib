using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
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

        public string ConnectionString { get { return _connectionString; } }
        public string SchemaStatement { get; private set; }
        public string DropSchemaStatement { get; private set; }
        public IDbConnection Connection { get; set; }

        public SqlExpressTestDatabase(string schema, string dropSchema)
        {
            SchemaStatement = schema;
            DropSchemaStatement = dropSchema;
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
        
        public void CreateSchema()
        {
            ExecuteQuery(SchemaStatement);
        }
        
        public void Dispose()
        {
            ExecuteQuery(DropSchemaStatement);
            Connection.Dispose();
            Connection = null;
            GC.Collect();
            SqlConnection.ClearAllPools();
        }

        public static void CreateDatabase()
        {  
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SetupConnection"].ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "CREATE DATABASE GriffinMvcContribSQLTest;";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception) { }
                }
            }
        }

        public void ExecuteQuery(string query)
        {
            Connection = new SqlConnection(_connectionString);
            Connection.Open();

            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
