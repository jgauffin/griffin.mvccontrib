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
        private const string CookieName = "GriffinLanguageSwitcher";

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
			if (filterContext.HttpContext.Request.QueryString["lang"] != null)
			{
				var lang = filterContext.HttpContext.Request.QueryString["lang"];
				Thread.CurrentThread.CurrentCulture =
					Thread.CurrentThread.CurrentUICulture =
						new CultureInfo(lang);

				SetCookie(filterContext.HttpContext.Response, lang);
			}
			else if (filterContext.RouteData.Values["lang"] != null)
			{
				var lang = filterContext.RouteData.Values["lang"].ToString();
				Thread.CurrentThread.CurrentCulture =
					Thread.CurrentThread.CurrentUICulture =
						new CultureInfo(lang);

				SetCookie(filterContext.HttpContext.Response, lang);
			}

			else if (filterContext.HttpContext.Request.Cookies[CookieName] != null)
			{
				Thread.CurrentThread.CurrentCulture =
					Thread.CurrentThread.CurrentUICulture = 
						new CultureInfo(filterContext.HttpContext.Request.Cookies[CookieName].Value);

			}
        }

    	private static void SetCookie(HttpResponseBase response, string language)
    	{
    		response.Cookies.Add(new HttpCookie(CookieName, language)
    		                     	{
    		                     		Expires = DateTime.Now.AddYears(1),
    		                     	});
    	}
    }
}