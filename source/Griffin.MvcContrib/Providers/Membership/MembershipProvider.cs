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

using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Web.Security;

namespace Griffin.MvcContrib.Providers.Membership
{
    /// <summary>
    /// A membership provider which uses different components to make it more SOLID.
    /// </summary>
    /// <seealso cref="IServiceLocator"/>
    /// <seealso cref="IAccountRepository"/>
    /// <seealso cref="IPasswordPolicy"/>
    /// <seealso cref="IPasswordStrategy"/>
    public class MembershipProvider : System.Web.Security.MembershipProvider
    {
        private static IServiceLocator _serviceLocator;
        private IAccountRepository _UserService;
        private IPasswordPolicy _passwordPolicy;
        private IPasswordStrategy _passwordStrategy;

        /// <summary>
        /// Gets a brief, friendly description suitable for display in administrative tools or other user interfaces (UIs).
        /// </summary>
        /// <returns>A brief, friendly description suitable for display in administrative tools or other UIs.</returns>
        public override string Description
        {
            get { return "A more friendly membership provider."; }
        }

        /*
        /// <summary>
        /// Gets the friendly name used to refer to the provider during configuration.
        /// </summary>
        /// <returns>The friendly name used to refer to the provider during configuration.</returns>
        public override string Name
        {
            get
            {
                return "GriffinMembershipProvider";
            }
        }
        */

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to retrieve their passwords.
        /// </summary>
        /// <returns>
        /// true if the membership provider is configured to support password retrieval; otherwise, false. The default is false.
        /// </returns>
        public override bool EnablePasswordRetrieval
        {
            get { return PasswordStrategy.IsPasswordsDecryptable && PasswordPolicy.IsPasswordRetrievalEnabled; }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
        /// <returns>
        /// true if the membership provider supports password reset; otherwise, false. The default is true.
        /// </returns>
        public override bool EnablePasswordReset
        {
            get { return PasswordPolicy.IsPasswordResetEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
        /// <returns>
        /// true if a password answer is required for password reset and retrieval; otherwise, false. The default is true.
        /// </returns>
        public override bool RequiresQuestionAndAnswer
        {
            get { return PasswordPolicy.IsPasswordQuestionRequired; }
        }

        /// <summary>
        /// The name of the application using the custom membership provider.
        /// </summary>
        /// <returns>
        /// The name of the application using the custom membership provider.
        /// </returns>
        public override string ApplicationName { get; set; }

        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </summary>
        /// <returns>
        /// The number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </returns>
        public override int MaxInvalidPasswordAttempts
        {
            get { return PasswordPolicy.MaxInvalidPasswordAttempts; }
        }

        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        /// <returns>
        /// The number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </returns>
        public override int PasswordAttemptWindow
        {
            get { return PasswordPolicy.PasswordAttemptWindow; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
        /// <returns>
        /// true if the membership provider requires a unique e-mail address; otherwise, false. The default is true.
        /// </returns>
        public override bool RequiresUniqueEmail
        {
            get { return UserService.IsUniqueEmailRequired; }
        }

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Web.Security.MembershipPasswordFormat"/> values indicating the format for storing passwords in the data store.
        /// </returns>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return PasswordStrategy.PasswordFormat; }
        }

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        /// <returns>
        /// The minimum length required for a password. 
        /// </returns>
        public override int MinRequiredPasswordLength
        {
            get { return PasswordPolicy.PasswordMinimumLength; }
        }

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        /// <returns>
        /// The minimum number of special characters that must be present in a valid password.
        /// </returns>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return PasswordPolicy.MinRequiredNonAlphanumericCharacters; }
        }

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        /// <returns>
        /// A regular expression used to evaluate a password.
        /// </returns>
        public override string PasswordStrengthRegularExpression
        {
            get { return PasswordPolicy.PasswordStrengthRegularExpression; }
        }

        protected IAccountRepository UserService
        {
            get
            {
                _UserService = _serviceLocator.Get<IAccountRepository>(this);
                if (_UserService == null)
                    throw new InvalidOperationException(
                        "You need to assign a locator to the ServiceLocator property and it should be able to lookup IAccountRepository.");
                return _UserService;
            }
        }

        protected IPasswordStrategy PasswordStrategy
        {
            get
            {
                _passwordStrategy = _serviceLocator.Get<IPasswordStrategy>(this);
                if (_passwordStrategy == null)
                    throw new InvalidOperationException(
                        "You need to assign a locator to the ServiceLocator property and it should be able to lookup IPasswordStrategy.");

                return _passwordStrategy;
            }
        }

        protected IPasswordPolicy PasswordPolicy
        {
            get
            {
                _passwordPolicy = _serviceLocator.Get<IPasswordPolicy>(this);
                if (_passwordPolicy == null)
                    throw new InvalidOperationException(
                        "You need to assign a locator to the ServiceLocator property and it should be able to lookup IPasswordPolicy.");

                return _passwordPolicy;
            }
        }

        public static void Configure(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator ?? NullLocator.Instance;
        }

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the information for the newly created user.
        /// </returns>
        /// <param name="username">The user name for the new user. </param><param name="password">The password for the new user. </param><param name="email">The e-mail address for the new user.</param><param name="passwordQuestion">The password question for the new user.</param><param name="passwordAnswer">The password answer for the new user</param><param name="isApproved">Whether or not the new user is approved to be validated.</param><param name="providerUserKey">The unique identifier from the membership data source for the user.</param><param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus"/> enumeration value indicating whether the user was created successfully.</param>
        public override MembershipUser CreateUser(string username, string password, string email,
                                                  string passwordQuestion, string passwordAnswer, bool isApproved,
                                                  object providerUserKey, out MembershipCreateStatus status)
        {
            if (UserService.IsUniqueEmailRequired && UserService.GetUserNameByEmail(email) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            if (UserService.Get(username) != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            ValidatePassword(username, password);


            var account = UserService.Create(providerUserKey, ApplicationName, username, email);
            var passwordInfo = new AccountPasswordInfo(username, password);
            account.Password = PasswordStrategy.Encrypt(passwordInfo);
            account.PasswordSalt = passwordInfo.PasswordSalt;

            status = UserService.Register(account);
            if (status == MembershipCreateStatus.Success)
                return CloneUser(account);

            return null;
        }

        protected MembershipUser CloneUser(IUserAccount account)
        {
            return new MembershipUser(Name, account.UserName, account.Id, account.Email,
                                      account.PasswordQuestion, account.Comment, account.IsApproved,
                                      account.IsLockedOut, account.CreatedAt, account.LastLoginAt,
                                      account.LastActivityAt, account.LastPasswordChangeAt, account.LastLockedOutAt);
        }

        /// <summary>
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <returns>
        /// true if the password question and answer are updated successfully; otherwise, false.
        /// </returns>
        /// <param name="username">The user to change the password question and answer for. </param><param name="password">The password for the specified user. </param><param name="newPasswordQuestion">The new password question for the specified user. </param><param name="newPasswordAnswer">The new password answer for the specified user. </param>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password,
                                                             string newPasswordQuestion, string newPasswordAnswer)
        {
            var account = UserService.Get(username);
            if (account == null)
                return false;

            var info = new AccountPasswordInfo(username, account.Password)
                           {
                               PasswordSalt = account.PasswordSalt
                           };
            if (PasswordStrategy.Compare(info, password))
                return false;

            account.PasswordQuestion = newPasswordAnswer;
            account.PasswordAnswer = newPasswordAnswer;
            UserService.Update(account);
            return true;
        }

        /// <summary>
        /// Gets the password for the specified user name from the data source.
        /// </summary>
        /// <returns>
        /// The password for the specified user name.
        /// </returns>
        /// <param name="username">The user to retrieve the password for. </param><param name="answer">The password answer for the user. </param>
        public override string GetPassword(string username, string answer)
        {
            if (!PasswordPolicy.IsPasswordRetrievalEnabled || !PasswordStrategy.IsPasswordsDecryptable)
                throw new ProviderException("Password retrieval is not supported");

            var account = UserService.Get(username);
            if (!account.PasswordAnswer.Equals(answer, StringComparison.OrdinalIgnoreCase))
                throw new MembershipPasswordException("Answer to Password question was incorrect.");

            return PasswordStrategy.Decrypt(account.Password);
        }

        /// <summary>
        /// Processes a request to update the password for a membership user.
        /// </summary>
        /// <returns>
        /// true if the password was updated successfully; otherwise, false.
        /// </returns>
        /// <param name="username">The user to update the password for. </param><param name="oldPassword">The current password for the specified user. </param><param name="newPassword">The new password for the specified user. </param>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var account = UserService.Get(username);
            var pwInfo = account.CreatePasswordInfo();
            if (!PasswordStrategy.Compare(pwInfo, oldPassword))
                return false;

            ValidatePassword(username, newPassword);

            account.Password = newPassword;
            account.Password = PasswordStrategy.Encrypt(pwInfo);
            UserService.Update(account);
            return true;
        }

        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <returns>
        /// The new password for the specified user.
        /// </returns>
        /// <param name="username">The user to reset the password for. </param><param name="answer">The password answer for the specified user. </param>
        public override string ResetPassword(string username, string answer)
        {
            if (!PasswordPolicy.IsPasswordResetEnabled)
                throw new NotSupportedException("Password reset is not supported.");

            var user = UserService.Get(username);
            if (PasswordPolicy.IsPasswordQuestionRequired && answer == null)
                throw new MembershipPasswordException("Password answer is empty and question/answer is required.");

            if (!user.PasswordAnswer.Equals(answer, StringComparison.OrdinalIgnoreCase))
                return null;

            var newPassword = PasswordStrategy.GeneratePassword(PasswordPolicy);

            ValidatePassword(username, newPassword);

            var info = new AccountPasswordInfo(username, newPassword);
            user.Password = PasswordStrategy.Encrypt(info);
            user.PasswordSalt = info.PasswordSalt;
            UserService.Update(user);
            return newPassword;
        }

        private void ValidatePassword(string username, string clearTextPassword)
        {
            if (!PasswordStrategy.IsValid(clearTextPassword, PasswordPolicy))
                throw new MembershipPasswordException("Password failed validation");

            var args = new ValidatePasswordEventArgs(username, clearTextPassword, false);
            OnValidatingPassword(args);
            if (args.FailureInformation != null)
                throw args.FailureInformation;
        }

        /// <summary>
        /// Updates information about a user in the data source.
        /// </summary>
        /// <param name="user">A <see cref="T:System.Web.Security.MembershipUser"/> object that represents the user to update and the updated information for the user. </param>
        public override void UpdateUser(MembershipUser user)
        {
            var account = UserService.Get(user.UserName);
            Merge(user, account);
            UserService.Update(account);
        }

        private void Merge(MembershipUser user, IUserAccount account)
        {
            account.Comment = user.Comment;
            account.IsApproved = user.IsApproved;
            account.Email = user.Email;
            account.PasswordQuestion = user.PasswordQuestion;
            account.IsLockedOut = user.IsLockedOut;
            //account.IsOnline = user.IsOnline;
            account.LastActivityAt = user.LastActivityDate;
            account.LastLockedOutAt = user.LastLockoutDate;
            account.LastPasswordChangeAt = user.LastPasswordChangedDate;
            account.Id = user.ProviderUserKey;
            account.UserName = user.UserName;
        }

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <returns>
        /// true if the specified username and password are valid; otherwise, false.
        /// </returns>
        /// <param name="username">The name of the user to validate. </param><param name="password">The password for the specified user. </param>
        public override bool ValidateUser(string username, string password)
        {
            var user = UserService.Get(username);
            if (user == null || user.IsLockedOut)
                return false;

            var passwordInfo = user.CreatePasswordInfo();
            var validated = PasswordStrategy.Compare(passwordInfo, password);
            if (validated)
            {
                user.LastLoginAt = DateTime.Now;
                user.FailedPasswordWindowStartedAt = DateTime.MinValue;
                user.FailedPasswordWindowAttemptCount = 0;
                UserService.Update(user);
                return true;
            }

            user.FailedPasswordWindowAttemptCount += 1;
            if (user.FailedPasswordWindowStartedAt == DateTime.MinValue)
                user.FailedPasswordAnswerWindowStartedAt = DateTime.Now;
            else if (DateTime.Now.Subtract(user.FailedPasswordAnswerWindowStartedAt).TotalMinutes >
                     PasswordPolicy.PasswordAttemptWindow)
            {
                user.IsLockedOut = true;
                user.LastLockedOutAt = DateTime.Now;
                UserService.Update(user);
            }

            return false;
        }

        /// <summary>
        /// Clears a lock so that the membership user can be validated.
        /// </summary>
        /// <returns>
        /// true if the membership user was successfully unlocked; otherwise, false.
        /// </returns>
        /// <param name="userName">The membership user whose lock status you want to clear.</param>
        public override bool UnlockUser(string userName)
        {
            var user = UserService.Get(userName);
            if (user == null)
                return false;

            user.IsLockedOut = false;
            user.FailedPasswordAnswerWindowAttemptCount = 0;
            user.FailedPasswordAnswerWindowStartedAt = DateTime.MinValue;
            user.FailedPasswordWindowAttemptCount = 0;
            user.FailedPasswordWindowStartedAt = DateTime.MinValue;
            UserService.Update(user);
            return true;
        }

        /// <summary>
        /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param><param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            var user = UserService.GetById(providerUserKey);
            if (user == null)
                return null;

            UpdateOnlineState(userIsOnline, user);

            return CloneUser(user);
        }

        private void UpdateOnlineState(bool userIsOnline, IUserAccount user)
        {
            if (!userIsOnline)
                return;

            user.LastActivityAt = DateTime.Now;
            //user.IsOnline = true;
            UserService.Update(user);
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        /// <param name="username">The name of the user to get information for. </param><param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user. </param>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            var user = UserService.Get(username);
            if (user == null)
                return null;

            UpdateOnlineState(userIsOnline, user);

            return CloneUser(user);
        }


        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <returns>
        /// The user name associated with the specified e-mail address. If no match is found, return null.
        /// </returns>
        /// <param name="email">The e-mail address to search for. </param>
        public override string GetUserNameByEmail(string email)
        {
            return UserService.GetUserNameByEmail(email);
        }

        /// <summary>
        /// Removes a user from the membership data source. 
        /// </summary>
        /// <returns>
        /// true if the user was successfully deleted; otherwise, false.
        /// </returns>
        /// <param name="username">The name of the user to delete.</param><param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return UserService.Delete(username, deleteAllRelatedData);
        }

        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param><param name="pageSize">The size of the page of results to return.</param><param name="totalRecords">The total number of matched users.</param>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var users = UserService.FindAll(pageIndex, pageSize, out totalRecords);
            return CloneUsers(users);
        }

        private MembershipUserCollection CloneUsers(IEnumerable<IUserAccount> users)
        {
            var members = new MembershipUserCollection();
            foreach (var user in users)
            {
                members.Add(CloneUser(user));
            }
            return members;
        }

        /// <summary>
        /// Gets the number of users currently accessing the application.
        /// </summary>
        /// <returns>
        /// The number of users currently accessing the application.
        /// </returns>
        public override int GetNumberOfUsersOnline()
        {
            return UserService.GetNumberOfUsersOnline();
        }

        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        /// <param name="usernameToMatch">The user name to search for.</param><param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param><param name="pageSize">The size of the page of results to return.</param><param name="totalRecords">The total number of matched users.</param>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
                                                                 out int totalRecords)
        {
            var users = UserService.FindByUserName(usernameToMatch, pageIndex, pageSize,
                                                   out totalRecords);
            return CloneUsers(users);
        }


        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        /// <param name="emailToMatch">The e-mail address to search for.</param><param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param><param name="pageSize">The size of the page of results to return.</param><param name="totalRecords">The total number of matched users.</param>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                                  out int totalRecords)
        {
            var users = UserService.FindByEmail(emailToMatch, pageIndex, pageSize,
                                                out totalRecords);
            return CloneUsers(users);
        }

        #region Nested type: NullLocator

        private class NullLocator : IServiceLocator
        {
            #region Implementation of IServiceLocator

            public static readonly NullLocator Instance = new NullLocator();

            public T Get<T>(object instance) where T : class
            {
                return null;
            }

            #endregion
        }

        #endregion
    }
}