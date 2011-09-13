using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Localization
{
    /// <summary>
    /// Uses a cookie and query string to turn into a language
    /// </summary>
    /// <remarks>
    /// Redirect to a page which has "?lang=XXX" in the query string to change language.
    /// </remarks>
    /// <example>
    /// <code>
    /// [Localized]
    /// public class BaseController : Controller
    /// {
    /// }
    /// 
    /// </code>
    /// </example>
    public class LocalizedAttribute : ActionFilterAttribute
    {
        private const string CookieName = "theLanguage";

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookie cookie = null;
            string language = "";

            if (filterContext.HttpContext.Request.QueryString["lang"] != null)
            {
                language = filterContext.HttpContext.Request.QueryString["lang"];
            }
            else
            {
                cookie = filterContext.HttpContext.Request.Cookies[CookieName];
                if (cookie != null)
                    language = cookie.Value;
                else if (filterContext.HttpContext.Request.UserLanguages != null)
                    language= filterContext.HttpContext.Request.UserLanguages[0];
                filterContext.RouteData.Values["lang"] = language;
            }

            SwitchLanguage(language);

            if (cookie != null && cookie.Value == language) 
                return;

            cookie = new HttpCookie(CookieName, Thread.CurrentThread.CurrentUICulture.Name)
                         {Expires = DateTime.Now.AddYears(1)};
            filterContext.HttpContext.Response.SetCookie(cookie);
        }

        private static void SwitchLanguage(string name)
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(name);
            }
            catch
            {}
        }
    }
}