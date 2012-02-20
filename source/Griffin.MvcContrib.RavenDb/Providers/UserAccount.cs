using System;
using System.Collections.Generic;
using Griffin.MvcContrib.Providers.Membership;
using Griffin.MvcContrib.Providers.Roles;
using Newtonsoft.Json;

namespace Griffin.MvcContrib.RavenDb.Providers
{
    public class UserAccount : IMembershipAccount, IUserWithRoles
    {
        private readonly List<string> _roles = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccount"/> class.
        /// </summary>
        public UserAccount()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccount"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        public UserAccount(IMembershipAccount user)
        {
            if (user == null) throw new ArgumentNullException("user");

            Copy(user);
        }

        public string Id { get; set; }

        #region IMembershipAccount Members

        public string UserName { get; set; }
        public string ApplicationName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public string PasswordSalt { get; set; }
        public string Comment { get; set; }
        public DateTime LastLoginAt { get; set; }
        public bool IsApproved { get; set; }
        public DateTime LastPasswordChangeAt { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastLockedOutAt { get; set; }
        public DateTime FailedPasswordWindowStartedAt { get; set; }
        public int FailedPasswordWindowAttemptCount { get; set; }
        public DateTime FailedPasswordAnswerWindowStartedAt { get; set; }
        public int FailedPasswordAnswerWindowAttemptCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastActivityAt { get; set; }

        #endregion

        #region IUserWithRoles Members

        /// <summary>
        /// Gets a list of all roles that the user is a member of.
        /// </summary>
        public IEnumerable<string> Roles
        {
            get { return _roles; }
        }

        /// <summary>
        /// Gets or sets key in the application db.
        /// </summary>
        public object ProviderUserKey { get; set; }

        /// <summary>
        /// Check if the user is a member of the specified role
        /// </summary>
        /// <param name="roleName">Role</param>
        /// <returns>true if user belongs to the role; otherwise false.</returns>
        public bool IsInRole(string roleName)
        {
            return _roles.Contains(roleName);
        }

        #endregion

        public void AddRole(string roleName)
        {
            _roles.Add(roleName);
        }

        public void RemoveRole(string roleName)
        {
            _roles.Remove(roleName);
        }

        public void Copy(IMembershipAccount account)
        {
            UserName = account.UserName;
            ApplicationName = account.ApplicationName;
            Comment = account.Comment;
            CreatedAt = account.CreatedAt;
            IsApproved = account.IsApproved;
            IsLockedOut = account.IsLockedOut;
            IsOnline = account.IsOnline;
            LastActivityAt = account.LastActivityAt;
            LastLockedOutAt = account.LastLockedOutAt;
            LastLoginAt = account.LastLoginAt;
            LastPasswordChangeAt = account.LastPasswordChangeAt;
            Email = account.Email;
            PasswordQuestion = account.PasswordQuestion;
            Password = account.Password;
            PasswordAnswer = account.PasswordAnswer;
            PasswordSalt = account.PasswordSalt;
            ProviderUserKey = account.ProviderUserKey;
            FailedPasswordAnswerWindowAttemptCount = account.FailedPasswordAnswerWindowAttemptCount;
            FailedPasswordAnswerWindowStartedAt = account.FailedPasswordAnswerWindowStartedAt;
            FailedPasswordWindowAttemptCount = account.FailedPasswordWindowAttemptCount;
            FailedPasswordWindowStartedAt = account.FailedPasswordWindowStartedAt;
        }
    }
}