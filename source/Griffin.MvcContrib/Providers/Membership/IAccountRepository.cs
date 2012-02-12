/*
 * Copyright (c) 2011, Jonas Gauffin. All rights reserved.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301 USA
 */

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
        /// Implementations should set the <see cref="IMembershipAccount.Id"/> property before returning.
        /// </remarks>
        MembershipCreateStatus Register(IMembershipAccount account);

        /// <summary>
        /// Fetch a user from the service.
        /// </summary>
        /// <param name="username">Unique user name</param>
        /// <returns>User if found; otherwise null.</returns>
        IMembershipAccount Get(string username);

        /// <summary>
        /// Update an existing user.
        /// </summary>
        /// <param name="account">Account being updated.</param>
        void Update(IMembershipAccount account);

        /// <summary>
        /// Get a user by using the implementation specific (your) Id.
        /// </summary>
        /// <param name="id">User identity specific for each account repository implementation</param>
        /// <returns>User if found; otherwise null.</returns>
        IMembershipAccount GetById(object id);

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
        IEnumerable<IMembershipAccount> FindAll(int pageIndex, int pageSize, out int totalRecords);

        /// <summary>
        /// Find new acounts that haven't been activated.
        /// </summary>
        /// <param name="pageIndex">zero based index</param>
        /// <param name="pageSize">Number of users per page</param>
        /// <param name="totalRecords">Total number of users</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        IEnumerable<IMembershipAccount> FindNewAccounts(int pageIndex, int pageSize, out int totalRecords);

        /// <summary>
        /// Find by searching for user name
        /// </summary>
        /// <param name="usernameToMatch">User name (or partial user name)</param>
        /// <param name="pageIndex">Zero based index</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="totalRecords">total number of records that partially matched the specified user name</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        IEnumerable<IMembershipAccount> FindByUserName(string usernameToMatch, int pageIndex, int pageSize,
                                                       out int totalRecords);

        /// <summary>
        /// Find by searching for the specified email
        /// </summary>
        /// <param name="emailToMatch">Number of users that have the specified email (no partial matches)</param>
        /// <param name="pageIndex">Zero based index</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="totalRecords">total number of records that matched the specified email</param>
        /// <returns>A collection of users or an empty collection if no users was found.</returns>
        IEnumerable<IMembershipAccount> FindByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                    out int totalRecords);

        /// <summary>
        /// Create a new membership account
        /// </summary>
        /// <param name="providerUserKey">Primary key in the data source</param>
        /// <param name="applicationName">Name of the application that the account is created for</param>
        /// <param name="username">User name</param>
        /// <param name="email">Email address</param>
        /// <returns>Created account</returns>
        IMembershipAccount Create(object providerUserKey, string applicationName, string username, string email);
    }
}