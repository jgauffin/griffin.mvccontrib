using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Griffin.MvcContrib
{
    /// <summary>
    /// Authorization attribute which checks the role names configured by the user.
    /// </summary>
    public class GriffinAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly string _roleConstantName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GriffinAuthorizeAttribute"/> class.
        /// </summary>
        /// <param name="roleConstantName">Name for one of the fields in <see cref="GriffinAdminRoles"/>.</param>
        public GriffinAuthorizeAttribute(string roleConstantName)
        {
            if (roleConstantName == null) throw new ArgumentNullException("roleConstantName");
            _roleConstantName = roleConstantName;
        }

        /// <summary>
        /// Called when authorization is required.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var roleName = GriffinAdminRoles.GetRoleFromName(_roleConstantName);
            if (roleName == null && filterContext.HttpContext.User.Identity.IsAuthenticated)
                return;

            if (filterContext.HttpContext.User.IsInRole(roleName))
                return;

            filterContext.Result = new HttpUnauthorizedResult();
        }

    }
}
