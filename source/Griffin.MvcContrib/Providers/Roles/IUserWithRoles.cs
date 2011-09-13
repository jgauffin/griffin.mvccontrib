using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.MvcContrib.Providers.Roles
{
    /// <summary>
    /// A user and it's roles
    /// </summary>
    public interface IUserWithRoles
    {
        /// <summary>
        /// Check if the user is a member of the specified role
        /// </summary>
        /// <param name="roleName">Role</param>
        /// <returns>true if user belongs to the role; otherwise false.</returns>
        bool IsInRole(string roleName);

        /// <summary>
        /// Gets a list of all roles that the user is a member of.
        /// </summary>
        IEnumerable<string> Roles { get; }
    }
}
