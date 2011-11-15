using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using Griffin.MvcContrib.Providers.Roles;
using Raven.Client;

namespace Griffin.MvcContrib.RavenDb.Providers
{
    public class RavenDbRoleRepository : IRoleRepository
    {
        private readonly IDocumentSession _session;

        public RavenDbRoleRepository(IDocumentSession session)
        {
            _session = session;
        }

        #region Implementation of IRoleRepository

        /// <summary>
        /// Get a user
        /// </summary>
        /// <param name="applicationName">Application that the request is for.</param>
        /// <param name="username">Account user name</param>
        /// <returns>User if found; otherwise null.</returns>
        public IUserWithRoles GetUser(string applicationName, string username)
        {
            return _session.Query<UserAccount>().Where(usr => usr.UserName == username).FirstOrDefault();
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="applicationName">Application that the request is for.</param>
        /// <param name="roleName">Name of role</param>
        public void CreateRole(string applicationName, string roleName)
        {
            _session.Store(new Role {ApplicationName = applicationName, Name = roleName});
            _session.SaveChanges();
        }


        /// <summary>
        /// Remove a role
        /// </summary>
        /// <param name="applicationName">Application that the request is for.</param>
        /// <param name="roleName">Role to remove</param>
        public void RemoveRole(string applicationName, string roleName)
        {
            var role = _session.Query<Role>().Where(r => r.Name == roleName);
            _session.Delete(role);
            _session.SaveChanges();
        }

        /// <summary>
        /// Add a user to an existing role
        /// </summary>
        /// <param name="applicationName">Application that the request is for.</param>
        /// <param name="roleName">Role that the user is going to be added to</param>
        /// <param name="username">User name</param>
        public void AddUserToRole(string applicationName, string roleName, string username)
        {
            var user = _session.Query<UserAccount>().Where(usr => usr.UserName == username).FirstOrDefault();
            if (user == null)
                throw new ProviderException("Failed to find user " + username);

            user.AddRole(roleName);
            _session.Store(user);
            _session.SaveChanges();
        }

        /// <summary>
        /// Remove an user from a role.
        /// </summary>
        /// <param name="applicationName">Application that the request is for.</param>
        /// <param name="roleName">Role that the user is going to be removed from.</param>
        /// <param name="username">User to remove</param>
        public void RemoveUserFromRole(string applicationName, string roleName, string username)
        {
            var user = _session.Query<UserAccount>().Where(usr => usr.UserName == username).FirstOrDefault();
            if (user == null)
                throw new ProviderException("Failed to find user " + username);

            user.RemoveRole(roleName);
            _session.Store(user);
            _session.SaveChanges();
        }

        /// <summary>
        /// Gets the role names.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns>A list with role names</returns>
        public IEnumerable<string> GetRoleNames(string applicationName)
        {
            return _session.Query<Role>().Select(r => r.Name).ToList();
        }

        /// <summary>
        /// Checks if a role exists in the specified application
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>true if found; otherwise false.</returns>
        public bool Exists(string applicationName, string roleName)
        {
            return _session.Query<Role>().Count(r => r.Name == roleName) != 0;
        }

        public int GetNumberOfUsersInRole(string applicationName, string roleName)
        {
            return _session.Query<UserAccount>().Count(usr => usr.Roles.Contains(roleName));
        }

        public IEnumerable<string> FindUsersInRole(string applicationName, string roleName, string userNameToMatch)
        {
            return
                _session.Query<UserAccount>().Where(
                    usr => usr.UserName.Contains(userNameToMatch) && usr.Roles.Contains(roleName)).Select(
                        usr => usr.UserName);
        }

        public IEnumerable<string> GetUsersInRole(string applicationName, string roleName)
        {
            return
                _session.Query<UserAccount>().Where(
                    usr => usr.Roles.Contains(roleName)).Select(
                        usr => usr.UserName);
        }

        #endregion
    }
}
