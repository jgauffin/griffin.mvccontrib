using System.Collections.Generic;

namespace Griffin.MvcContrib.Providers.Roles
{
    /// <summary>
    /// Membership role
    /// </summary>
    public interface IRole
    {
        /// <summary>
        /// Gets number of users in role
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets role name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets all users that have this role
        /// </summary>
        IEnumerable<string> Users { get; set; }
    }
}