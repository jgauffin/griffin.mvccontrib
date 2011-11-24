using System;
using System.Collections.Generic;
using System.Web.Security;
using Griffin.MvcContrib.Providers.Membership;
using Griffin.MvcContrib.Providers.Roles;
using Newtonsoft.Json;

namespace Griffin.MvcContrib.RavenDb.Providers
{
    public class UserAccount : IMembershipAccount, IUserWithRoles
    {
		public UserAccount()
		{
			
		}
		public UserAccount(MembershipUser user)
		{
			UserName = user.UserName;
			Email = user.Email;
			PasswordQuestion = user.PasswordQuestion;
			
		}
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
        public DateTime FailedPasswordWindowStartedAt { get;  set; }
        public int FailedPasswordWindowAttemptCount { get;  set; }
        public DateTime FailedPasswordAnswerWindowStartedAt { get;  set; }
        public int FailedPasswordAnswerWindowAttemptCount { get;  set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastActivityAt { get; set; }

        [JsonIgnore]
         object IMembershipAccount.Id { get { return UserName; } set{} }

        /// <summary>
        /// Gets a list of all roles that the user is a member of.
        /// </summary>
        public IEnumerable<string> Roles
        {
            get { return _roles; }
        }

        private List<string> _roles = new List<string>();

        /// <summary>
        /// Check if the user is a member of the specified role
        /// </summary>
        /// <param name="roleName">Role</param>
        /// <returns>true if user belongs to the role; otherwise false.</returns>
        public bool IsInRole(string roleName)
        {
            return _roles.Contains(roleName);
        }

        public void AddRole(string roleName)
        {
            _roles.Add(roleName);
        }

        public void RemoveRole(string roleName)
        {
            _roles.Remove(roleName);
        }
    }

   
}