using System;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Caching
{
    /// <summary>
    /// Do not cache anything
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoCacheAttribute : OutputCacheAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoCacheAttribute"/> class.
        /// </summary>
        public NoCacheAttribute()
        {
            NoStore = true;
            Duration = 0;
            VaryByParam = "*";
        }
    }
}