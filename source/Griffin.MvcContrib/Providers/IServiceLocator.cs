using Griffin.MvcContrib.Providers.Membership;
using Griffin.MvcContrib.Providers.Roles;

namespace Griffin.MvcContrib.Providers
{
    /// <summary>
    /// Used by the <see cref="MembershipProvider"/> and <see cref="RoleProvider"/> to locate services.
    /// </summary>
    /// <remarks>
    /// 
    /// Should be able too lookup 
    /// <list type="bullet">
    /// <item>
    /// <see cref="IPasswordPolicy"/>
    /// </item>
    /// <item>
    /// <see cref="IPasswordStrategy"/> 
    /// </item>
    /// <item>
    /// <see cref="IAccountRepository"/>
    /// </item>
    /// </list>
    /// </remarks>
    public interface IServiceLocator 
    {
        /// <summary>
        /// Get one of the services needed by <see cref="MembershipProvider"/>.
        /// </summary>
        /// <typeparam name="T">Service to find</typeparam>
        /// <param name="provider">The provider requesting a service.</param>
        /// <returns>Must return an instance</returns>
        /// <remarks>
        /// You can safely ignore the instance parameter if you only got one provider instance
        /// in the system. It's used to be able to return different services per instance.
        /// </remarks>
        T Get<T>(object provider) where T : class;
    }
}