using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Autofac;
using Autofac.Integration.Mvc;
using Griffin.MvcContrib.SqlServer.Localization;

namespace SqlServerLocalization.Modules
{
    public class SqlServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConnectionScope>().As<ILocalizationDbContext>().InstancePerHttpRequest();
            builder.RegisterType<SqlLocalizedTypesRepository>().AsImplementedInterfaces().InstancePerHttpRequest();
            builder.RegisterType<SqlLocalizedViewsRepository>().AsImplementedInterfaces().InstancePerHttpRequest();
            base.Load(builder);
        }
    }

    public class ConnectionScope : ILocalizationDbContext, IDisposable
    {
        private ConnectionStringSettings _conString;
        private readonly DbProviderFactory _factory;
        private IDbConnection _connection;

        public ConnectionScope()
        {
            string connectionStringName = "DemoDb";

            if (connectionStringName == null) throw new ArgumentNullException("connectionStringName");
            _conString = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (_conString == null)
                throw new InvalidOperationException("Failed to find connection string named " + connectionStringName + " in your configuration file (should exist in the <connectionStrings> element)");

            _factory = DbProviderFactories.GetFactory(_conString.ProviderName);
            if (_factory == null)
                throw new InvalidOperationException("Failed to find provider " + _conString.ProviderName + " which is used in connection string " + connectionStringName);
        }

        public string ChangePrefix(string sql)
        {
            return sql;
        }

        public IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {

                    _connection = _factory.CreateConnection();
                    if (_connection == null)
                        throw new InvalidOperationException("Factory " + _conString.ProviderName + " failed to create a new connection object.");

                    _connection.ConnectionString = _conString.ConnectionString;
                    _connection.Open();
                }

                if (_connection.State == ConnectionState.Closed)
                    Debugger.Break();
                return _connection;
            }
        }

        public char ParameterPrefix
        {
            get { return '@'; }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}