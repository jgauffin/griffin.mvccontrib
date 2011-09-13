using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Localization
{
    /// <summary>
    /// Make the cache language sensitive.
    /// </summary>
    /// <remarks>
    /// You need to add the following method you your global.asax:
    /// <code>
    /// public override string GetVaryByCustomString(HttpContext context, string custom)
    /// {
    ///     string lang;
    ///     return LocalizedCacheAttribute.GetVaryByCustomString(context, custom, out lang)
    ///                ? lang
    ///                : base.GetVaryByCustomString(context, custom);
    /// }
    /// </code>
    /// </remarks>
    public class LocalizedCacheAttribute : OutputCacheAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedCacheAttribute"/> class.
        /// </summary>
        public LocalizedCacheAttribute()
        {
            VaryByCustom = "lang";
        }

        /// <summary>
        /// Method that determines if the cahing is OK or not
        /// </summary>
        /// <param name="context">Current HttpContext.</param>
        /// <param name="value">Value to vary by.</param>
        /// <param name="lang">The "vary" result to use.</param>
        /// <returns><c>true</c> if the vary request was handled; otherwise <c>false</c>.</returns>
        public static bool GetVaryByCustomString(HttpContext context, string value, out string lang)
        {
            if (value.Equals("lang"))
            {
                lang = Thread.CurrentThread.CurrentUICulture.Name;
                return true;
            }

            lang = value;
            return false;
        }
    }
}
