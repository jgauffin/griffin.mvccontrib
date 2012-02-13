using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using Griffin.MvcContrib.Providers.Membership;
using Griffin.MvcContrib.RavenDb.Providers;
using Raven.Client;

namespace Griffin.MvcContrib.RavenDb.Providers
{
    public class RavenDbAccountRepository : IAccountRepository
    {
        private readonly IDocumentSession _documentSession;

        public RavenDbAccountRepository(IDocumentSession documentSession)
        {
            this._documentSession = documentSession;
        }

        #region Implementation of IAccountRepository

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
            account.Id = account.UserName;
            _documentSession.Store(account);
            _documentSession.SaveChanges();
            return MembershipCreateStatus.Success;
        }

        /// <summary>
        /// Fetch a user from the service.
        /// </summary>
        /// <param name="username">Unique user name</param>
        /// <returns>User if found; otherwise null.</returns>
        public IMembershipAccount Get(string username)
        {
            return _documentSession.Query<UserAccount>().Where(user => user.UserName == username).FirstOrDefault();
        }

        /// <summary>
        /// Update an existing user.
        /// </summary>
        /// <param name="account">Account being updated.</param>
        public void Update(IMembershipAccount account)
        {
            _documentSession.Store(account);
            _documentSession.SaveChanges();
        }

        /// <summary>
        /// Get a user by using the implementation specific (your) Id.
        /// </summary>
        /// <param name="id">User identity specific for each account repository implementation</param>
        /// <returns>User if found; otherwise null.</returns>
        public IMembershipAccount GetById(object id)
        {
            return _documentSession.Query<UserAccount>().Where(user => user.UserName == (string)id).FirstOrDefault();
        }

        /// <summary>
        /// Translate an email into a user name.
        /// </summary>
        /// <param name="email">Email to lookup</param>
        /// <returns>User name if the specified email was found; otherwise null.</returns>
        public string GetUserNameByEmail(string email)
        {
            return _documentSession.Query<UserAccount>().Where(user => user.Email == email).Select(user => user.UserName).FirstOrDefault();
        }

        /// <summary>
        /// Delete a user from the database.
        /// </summary>
        /// <param name="username">Unique user name</param>
        /// <param name="deleteAllRelatedData">Delete information from all other tables etc</param>
        /// <returns>true if was removed successfully; otherwise false.</returns>
        public bool Delete(string username, bool deleteAllRelatedData)
        {
            var dbUser = _documentSession.Query<UserAccount>().Where(user => user.UserName == username).FirstOrDefault();
            if (dbUser == null)
                return true;

            _documentSession.Delete(dbUser);

            try
            {
                Deleted(this, new DeletedEventArgs(dbUser));
                _documentSession.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public event EventHandler<DeletedEventArgs> Deleted = delegate { };

        /// <summary>
        /// Get number of users that are online
        /// </summary>
        /// <returns>Number of online users</returns>
        public int GetNumberOfUsersOnline()
        {
            return _documentSession.Query<UserAccount>().Where(user => user.IsOnline).Count();
        }

        /// <summary>
        /// Find all users
        /// </summary>
        /// <param name="pageIndex">One based index</param>
        /// <param name="pageSize">Number of users per page</param>
        /// <param name="totalRecords">Total number of users</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        public IEnumerable<IMembershipAccount> FindAll(int pageIndex, int pageSize, out int totalRecords)
        {
            IQueryable<UserAccount> query = _documentSession.Query<UserAccount>();
            query = CountAndPageQuery(pageIndex, pageSize, out totalRecords, query);
            return query.ToList();
        }

        /// <summary>
        /// Find new acounts that haven't been activated.
        /// </summary>
        /// <param name="pageIndex">zero based index</param>
        /// <param name="pageSize">Number of users per page</param>
        /// <param name="totalRecords">Total number of users</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        public IEnumerable<IMembershipAccount> FindNewAccounts(int pageIndex, int pageSize, out int totalRecords)
        {
            IQueryable<UserAccount> query = _documentSession.Query<UserAccount>().Where(p => p.IsApproved==false);
            query = CountAndPageQuery(pageIndex, pageSize, out totalRecords, query);
            return query.ToList();
        }

        /// <summary>
        /// Find by searching for user name
        /// </summary>
        /// <param name="usernameToMatch">User name (or partial user name)</param>
        /// <param name="pageIndex">Zero based index</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="totalRecords">total number of records that partially matched the specified user name</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        public IEnumerable<IMembershipAccount> FindByUserName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var query = _documentSession.Query<UserAccount>().Where(user => user.UserName.Contains(usernameToMatch));
            query = CountAndPageQuery(pageIndex, pageSize, out totalRecords, query);
            return query.ToList();
        }

        private IQueryable<UserAccount> CountAndPageQuery(int pageIndex, int pageSize, out int totalRecords, IQueryable<UserAccount> query)
        {
            totalRecords = query.Count();
            query = pageIndex == 1
                        ? _documentSession.Query<UserAccount>().Take(pageSize)
                        : _documentSession.Query<UserAccount>().Skip((pageIndex - 1)*pageSize).Take(pageSize);
            return query;
        }

        /// <summary>
        /// Find by searching for the specified email
        /// </summary>
        /// <param name="emailToMatch">Number of users that have the specified email (no partial matches)</param>
        /// <param name="pageIndex">Zero based index</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="totalRecords">total number of records that matched the specified email</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        public IEnumerable<IMembershipAccount> FindByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var query = _documentSession.Query<UserAccount>().Where(user => user.Email == emailToMatch);
            query = CountAndPageQuery(pageIndex, pageSize, out totalRecords, query);
            return query.ToList();
        }

        public IMembershipAccount Create(object providerUserKey, string applicationName, string username, string email)
        {
            var account = new UserAccount
                              {
                                  ApplicationName = applicationName,
                                  UserName = username,
                                  Email = email,
                                  CreatedAt = DateTime.Now
                              };
            return account;
        }

        #endregion
    }

    public class DeletedEventArgs : EventArgs
    {
        public DeletedEventArgs(IMembershipAccount account)
        {
            
        }
    }
}