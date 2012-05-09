using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Griffin.MvcContrib.VirtualPathProvider;

namespace Griffin.MvcContrib.Plugins
{
    /// <summary>
    /// Locator which loads views using the project structure to enable runtime view edits.
    /// </summary>
    /// <remarks>
    /// Works as long as you have used the structure which is described in the namespace documentation.
    /// </remarks>
    /// <seealso cref="Griffin.MvcContrib.Plugins"/>
    public class PluginFileLocator : IViewFileLocator
    {
        private readonly string _basePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginFileLocator"/> class.
        /// </summary>
        public PluginFileLocator()
        {
            _basePath = Path.GetFullPath(HostingEnvironment.MapPath("~") + @"..\Plugins\");
        }

        #region IViewFileLocator Members

        /// <summary>
        /// Get full path to a file
        /// </summary>
        /// <param name="uri">Requested uri</param>
        /// <returns>
        /// Full disk path if found; otherwise null.
        /// </returns>
        public string GetFullPath(string uri)
        {
            var fixedUri = uri;
            if (fixedUri.StartsWith("~"))
                fixedUri = VirtualPathUtility.ToAbsolute(uri);
            if (!fixedUri.StartsWith("/Areas", StringComparison.OrdinalIgnoreCase))
                return null;

            // extract area name:
            var pos = fixedUri.IndexOf('/', 7);
            if (pos == -1)
                return null;
            var areaName = fixedUri.Substring(7, pos - 7);

            var path = string.Format("{0}{1}\\Plugin.{1}{2}", _basePath, areaName, fixedUri.Replace('/', '\\'));
            return File.Exists(path) ? path : null;
        }

        #endregion
    }
}