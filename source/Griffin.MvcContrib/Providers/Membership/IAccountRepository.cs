using System.Collections.Generic;
using System.Web.Security;

namespace Griffin.MvcContrib.Providers.Membership
{
    /// <summary>
    /// Repository for user accounts
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Gets whether all users must have unique email addresses.
        /// </summary>
        bool IsUniqueEmailRequired { get; }

        /// <summary>
        /// Register a new account.
        /// </summary>
        /// <param name="account">Acount to register</param>
        /// <returns>Result indication</returns>
        /// <remarks>
        /// Implementations should set the <see cref="IUserAccount.Id"/> property before returning.
        /// </remarks>
        MembershipCreateStatus Register(IUserAccount account);

        /// <summary>
        /// Fetch a user from the service.
        /// </summary>
        /// <param name="username">Unique user name</param>
        /// <returns>User if found; otherwise null.</returns>
        IUserAccount Get(string username);

        /// <summary>
        /// Update an existing user.
        /// </summary>
        /// <param name="account">Account being updated.</param>
        void Update(IUserAccount account);

        /// <summary>
        /// Get a user by using the implementation specific (your) Id.
        /// </summary>
        /// <param name="id">User identity specific for each account repository implementation</param>
        /// <returns>User if found; otherwise null.</returns>
        IUserAccount GetById(object id);

        /// <summary>
        /// Translate an email into a user name.
        /// </summary>
        /// <param name="email">Email to lookup</param>
        /// <returns>User name if the specified email was found; otherwise null.</returns>
        string GetUserNameByEmail(string email);

        /// <summary>
        /// Delete a user from the database.
        /// </summary>
        /// <param name="username">Unique user name</param>
        /// <param name="deleteAllRelatedData">Delete information from all other tables etc</param>
        /// <returns>true if was removed successfully; otherwise false.</returns>
        bool Delete(string username, bool deleteAllRelatedData);

        /// <summary>
        /// Get number of users that are online
        /// </summary>
        /// <returns>Number of online users</returns>
        int GetNumberOfUsersOnline();

        /// <summary>
        /// Find all users
        /// </summary>
        /// <param name="pageIndex">zero based index</param>
        /// <param name="pageSize">Number of users per page</param>
        /// <param name="totalRecords">Total number of users</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        IEnumerable<IUserAccount> FindAll(int pageIndex, int pageSize, out int totalRecords);

        /// <summary>
        /// Find by searching for user name
        /// </summary>
        /// <param name="usernameToMatch">User name (or partial user name)</param>
        /// <param name="pageIndex">Zero based index</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="totalRecords">total number of records that partially matched the specified user name</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        IEnumerable<IUserAccount> FindByUserName(string usernameToMatch, int pageIndex, int pageSize,
                                                out int totalRecords);

        /// <summary>
        /// Find by searching for the specified email
        /// </summary>
        /// <param name="emailToMatch">Number of users that have the specified email (no partial matches)</param>
        /// <param name="pageIndex">Zero based index</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="totalRecords">total number of records that matched the specified email</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        IEnumerable<IUserAccount> FindByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords);

        IUserAccount Create(object providerUserKey, string applicationName, string username, string email);
    }
}