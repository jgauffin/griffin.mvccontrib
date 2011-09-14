using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Web.Security;

namespace Griffin.MvcContrib.Providers.Membership.SqlRepository
{
    /// <summary>
    /// SQL repository.
    /// </summary>
    /// <remarks>Uses a connection string named "sqlmembership" as default.</remarks>
    public class SqlAccountRepository : IAccountRepository
    {
        private readonly ISqlAdapter _sqlAdapter;
        private ConnectionStringSettings _connectionString;
        private DbProviderFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAccountRepository"/> class.
        /// </summary>
        /// <param name="sqlAdapter">Used to modify SQL to vendor specific statements.</param>
        public SqlAccountRepository(ISqlAdapter sqlAdapter)
        {
            _sqlAdapter = sqlAdapter;
        }

        #region IAccountRepository Members

        /// <summary>
        /// Gets whether all users must have unique email addresses.
        /// </summary>
        public bool IsUniqueEmailRequired { get; set; }

        /// <summary>
        /// Register a new account.
        /// </summary>
        /// <param name="account">Acount to register</param>
        /// <returns>Result indication</returns>
        /// <remarks>
        /// Implementations should set the <see cref="IMembershipAccount.Id"/> property before returning.
        /// </remarks>
        public MembershipCreateStatus Register(IMembershipAccount account)
        {
            const string sql =
                @"INSERT INTO membership_accounts (
                     application_name,
                     comment,
                     created_at,
                     email,
                     is_approved,
                     is_locked_out,
                     last_activity_at,
                     password,
                     password_answer,
                     password_question,
                     username
                ) VALUES (
                     @application_name,
                     @comment,
                     @created_at,
                     @email,
                     @is_approved,
                     @is_locked_out,
                     @last_activity_at,
                     @password,
                     @password_answer,
                     @password_question,
                     @username
                )";


            try
            {
                using (var connection = CreateAndOpenConnection())
                {
                    using (var cmd = connection.CreateCommand(sql))
                    {
                        cmd.AddParameter("application_name", account.ApplicationName);
                        cmd.AddParameter("comment", account.Comment);
                        cmd.AddParameter("created_at", account.CreatedAt.SqlSafe());
                        cmd.AddParameter("email", account.Email);
                        cmd.AddParameter("failed_password_answer_window_attempt_count",
                                         account.FailedPasswordAnswerWindowAttemptCount);
                        cmd.AddParameter("failed_password_answer_window_started_at",
                                         account.FailedPasswordAnswerWindowStartedAt);
                        cmd.AddParameter("failed_password_window_attempt_count",
                                         account.FailedPasswordWindowAttemptCount);
                        cmd.AddParameter("failed_password_window_started_at", account.FailedPasswordWindowStartedAt);
                        cmd.AddParameter("is_approved", account.IsApproved);
                        cmd.AddParameter("is_locked_out", account.IsLockedOut);
                        cmd.AddParameter("last_activity_at", account.LastActivityAt);
                        cmd.AddParameter("last_locked_out_at", account.LastLockedOutAt);
                        cmd.AddParameter("last_login_at", account.LastLoginAt);
                        cmd.AddParameter("last_password_change_at", account.LastPasswordChangeAt);
                        cmd.AddParameter("password", account.Password);
                        cmd.AddParameter("password_answer", account.PasswordAnswer);
                        cmd.AddParameter("password_question", account.PasswordQuestion);
                        cmd.AddParameter("username", account.UserName);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                return MembershipCreateStatus.ProviderError;
            }

            return MembershipCreateStatus.Success;
        }

        /// <summary>
        /// Fetch a user from the service.
        /// </summary>
        /// <param name="username">Unique user name</param>
        /// <returns>User if found; otherwise null.</returns>
        public IMembershipAccount Get(string username)
        {
            using (var connection = CreateAndOpenConnection())
            {
                const string sql = "SELECT * FROM membership_accounts WHERE username=@username";
                using (var cmd = connection.CreateCommand(sql))
                {
                    cmd.AddParameter("username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        var account = CreateAccount();
                        MapAccount(reader, account);
                        return account;
                    }
                }
            }
        }

        /// <summary>
        /// Update an existing user.
        /// </summary>
        /// <param name="account">Account being updated.</param>
        public void Update(IMembershipAccount account)
        {
            const string sql =
                @"UPDATE membership_accounts SET 
                        application_name = @application_name,
                        comment = @comment,
                        created_at = @created_at,
                        email = @email,
                        is_approved = @is_approved,
                        is_locked_out = @is_locked_out,
                        last_activity_at = @last_activity_at,
                        password = @password,
                        password_answer = @password_answer,
                        password_question = @password_question,
                        username = @username
                    WHERE id = @id";

            using (var connection = CreateAndOpenConnection())
            {
                using (var cmd = connection.CreateCommand(sql))
                {
                    cmd.AddParameter("application_name", account.ApplicationName);
                    cmd.AddParameter("comment", account.Comment);
                    cmd.AddParameter("created_at", account.CreatedAt.SqlSafe());
                    cmd.AddParameter("email", account.Email);
                    cmd.AddParameter("failed_password_answer_window_attempt_count",
                                     account.FailedPasswordAnswerWindowAttemptCount);
                    cmd.AddParameter("failed_password_answer_window_started_at",
                                     account.FailedPasswordAnswerWindowStartedAt);
                    cmd.AddParameter("failed_password_window_attempt_count", account.FailedPasswordWindowAttemptCount);
                    cmd.AddParameter("failed_password_window_started_at", account.FailedPasswordWindowStartedAt);
                    cmd.AddParameter("is_approved", account.IsApproved);
                    cmd.AddParameter("is_locked_out", account.IsLockedOut);
                    cmd.AddParameter("last_activity_at", account.LastActivityAt);
                    cmd.AddParameter("last_locked_out_at", account.LastLockedOutAt);
                    cmd.AddParameter("last_login_at", account.LastLoginAt);
                    cmd.AddParameter("last_password_change_at", account.LastPasswordChangeAt);
                    cmd.AddParameter("password", account.Password);
                    cmd.AddParameter("password_answer", account.PasswordAnswer);
                    cmd.AddParameter("password_question", account.PasswordQuestion);
                    cmd.AddParameter("username", account.UserName);
                    cmd.AddParameter("id", account.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Get a user by using the implementation specific (your) Id.
        /// </summary>
        /// <param name="id">User identity specific for each account repository implementation</param>
        /// <returns>User if found; otherwise null.</returns>
        public IMembershipAccount GetById(object id)
        {
            using (var connection = CreateAndOpenConnection())
            {
                const string sql = "SELECT * FROM membership_accounts WHERE id=@id";
                using (var cmd = connection.CreateCommand(sql))
                {
                    cmd.AddParameter("id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        var account = CreateAccount();
                        MapAccount(reader, account);
                        return account;
                    }
                }
            }
        }

        /// <summary>
        /// Translate an email into a user name.
        /// </summary>
        /// <param name="email">Email to lookup</param>
        /// <returns>User name if the specified email was found; otherwise null.</returns>
        public string GetUserNameByEmail(string email)
        {
            using (var connection = CreateAndOpenConnection())
            {
                const string sql = "SELECT username FROM membership_accounts WHERE email=@email";
                using (var cmd = connection.CreateCommand(sql))
                {
                    cmd.AddParameter("email", email);
                    return (string) cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Delete a user from the database.
        /// </summary>
        /// <param name="username">Unique user name</param>
        /// <param name="deleteAllRelatedData">Delete information from all other tables etc</param>
        /// <returns>true if was removed successfully; otherwise false.</returns>
        public bool Delete(string username, bool deleteAllRelatedData)
        {
            using (var connection = CreateAndOpenConnection())
            {
                const string sql = "DELETE FROM membership_accounts WHERE username=@username";
                using (var cmd = connection.CreateCommand(sql))
                {
                    cmd.AddParameter("username", username);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Get number of users that are online
        /// </summary>
        /// <returns>Number of online users</returns>
        public int GetNumberOfUsersOnline()
        {
            using (var connection = CreateAndOpenConnection())
            {
                const string sql = "SELECT count(*) FROM membership_accounts WHERE last_activity_at>=@mindate";
                using (var cmd = connection.CreateCommand(sql))
                {
                    cmd.AddParameter("mindate", DateTime.Now.AddMinutes(10));
                    return (int) cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Find all users
        /// </summary>
        /// <param name="pageIndex">zero based index</param>
        /// <param name="pageSize">Number of users per page</param>
        /// <param name="totalRecords">Total number of users</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        public IEnumerable<IMembershipAccount> FindAll(int pageIndex, int pageSize, out int totalRecords)
        {
            return Find(cmd => { }, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Find by searching for user name
        /// </summary>
        /// <param name="usernameToMatch">User name (or partial user name)</param>
        /// <param name="pageIndex">Zero based index</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="totalRecords">total number of records that partially matched the specified user name</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        public IEnumerable<IMembershipAccount> FindByUserName(string usernameToMatch, int pageIndex, int pageSize,
                                                              out int totalRecords)
        {
            return Find(cmd =>
                            {
                                cmd.CommandText += " WHERE username LIKE @username";
                                cmd.AddParameter("username", usernameToMatch + "%");
                            }, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Find by searching for the specified email
        /// </summary>
        /// <param name="emailToMatch">Number of users that have the specified email (no partial matches)</param>
        /// <param name="pageIndex">Zero based index</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="totalRecords">total number of records that matched the specified email</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        public IEnumerable<IMembershipAccount> FindByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                           out int totalRecords)
        {
            return Find(cmd =>
                            {
                                cmd.CommandText += " WHERE email = @email";
                                cmd.AddParameter("email", emailToMatch);
                            }, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Creates the specified provider user key.
        /// </summary>
        /// <param name="providerUserKey">The provider user key.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="username">The username.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public IMembershipAccount Create(object providerUserKey, string applicationName, string username, string email)
        {
            var account = CreateAccount();
            account.Id = providerUserKey;
            account.ApplicationName = applicationName;
            account.UserName = username;
            account.Email = email;
            return account;
        }

        #endregion

        /// <summary>
        /// Breaks SRP, but nothing major since the whole provider can be mocked.
        /// </summary>
        /// <returns></returns>
        private IDbConnection CreateAndOpenConnection()
        {
            if (_connectionString == null)
            {
                _connectionString = ConfigurationManager.ConnectionStrings["sqlaccountrepository"];
                if (_connectionString == null)
                    throw new ConfigurationErrorsException(
                        "Failed to find connection string for the SqlAccountRepository. Create one named 'sqlaccountrepository' in the connectionStrings section of web.config.");
            }

            if (_factory == null)
            {
                _factory = DbProviderFactories.GetFactory(_connectionString.ProviderName);
                if (_factory == null)
                    throw new ConfigurationErrorsException("Failed to find a provider factory named '" +
                                                           _connectionString.ProviderName + "'.");
            }

            var connection = _factory.CreateConnection();
            if (connection == null)
                throw new InvalidOperationException("Provider factory did not create an connection");

            connection.ConnectionString = _connectionString.ConnectionString;
            connection.Open();
            return connection;
        }

        internal static void MapAccount(IDataRecord reader, IMembershipAccount account)
        {
            account.ApplicationName = reader["application_name"].ToString();
            account.Comment = reader["application_name"].ToString();
            account.CreatedAt = reader.FromSqlDate("created_at");
            account.FailedPasswordAnswerWindowAttemptCount =
                (int) reader["failed_password_answer_window_attempt_clount"];
            account.FailedPasswordAnswerWindowStartedAt = reader.FromSqlDate("failed_password_answer_window_started_at");
            account.FailedPasswordWindowAttemptCount = (int) reader["failed_password_window_attempt_count"];
            account.FailedPasswordWindowStartedAt = reader.FromSqlDate("failed_password_window_started_at");
            account.Id = reader["id"].ToString();
            account.IsApproved = (int) reader["is_approvied"] == 1;
            account.IsLockedOut = (int) reader["is_locked_out"] == 1;
            account.LastActivityAt = reader.FromSqlDate("last_activity_at");
            account.LastLockedOutAt = reader.FromSqlDate("last_locked_out_at");
            account.LastLoginAt = reader.FromSqlDate("last_login_at");
            account.LastPasswordChangeAt = reader.FromSqlDate("last_password_change_at");
            account.Password = reader["password"].ToString();
            account.PasswordAnswer = reader["password_answer"].ToString();
            account.PasswordQuestion = reader["password_question"].ToString();
            account.PasswordSalt = reader["password_salt"].ToString();
            account.UserName = reader["username"].ToString();
        }

        /// <summary>
        /// Create a membership account implementation
        /// </summary>
        /// <returns>Created account</returns>
        protected IMembershipAccount CreateAccount()
        {
            return new MembershipAccount();
        }

        private IEnumerable<IMembershipAccount> Find(Action<IDbCommand> prepareCommand, int pageIndex, int pageSize,
                                                     out int totalRecords)
        {
            using (var connection = CreateAndOpenConnection())
            {
                var sql = "SELECT count(*) FROM membership_accounts";
                using (var cmd = connection.CreateCommand(sql))
                {
                    prepareCommand(cmd);
                    totalRecords = (int) cmd.ExecuteScalar();
                }

                sql = "SELECT * FROM membership_accounts";
                sql = _sqlAdapter.PageSql(sql, pageIndex, pageSize);
                using (var cmd = connection.CreateCommand(sql))
                {
                    prepareCommand(cmd);
                    using (var reader = cmd.ExecuteReader())
                    {
                        var accounts = new List<IMembershipAccount>();
                        while (reader.Read())
                        {
                            var account = CreateAccount();
                            MapAccount(reader, account);
                            accounts.Add(account);
                        }
                        return accounts;
                    }
                }
            }
        }
    }
}