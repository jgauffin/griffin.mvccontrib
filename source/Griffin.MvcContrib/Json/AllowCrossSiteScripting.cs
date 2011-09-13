using System;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Allow cross site scripting for a controller or an action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        private readonly string _allowOrigin;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllowCrossSiteJsonAttribute"/> class.
        /// </summary>
        /// <remarks>Allows cross site scripting from everywhere.</remarks>
        public AllowCrossSiteJsonAttribute()
        {
            _allowOrigin = "*";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AllowCrossSiteJsonAttribute"/> class.
        /// </summary>
        /// <param name="allowOrigin">Allows cross site scripting from the specified domain only.</param>
        public AllowCrossSiteJsonAttribute(string allowOrigin)
        {
            _allowOrigin = allowOrigin;
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", _allowOrigin);
            base.OnActionExecuting(filterContext);
        }
    }

}
