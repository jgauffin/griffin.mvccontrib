using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private IEnumerable<string> _allowedFileExtensions;

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
        /// Full disk path if found; otherwise <c>null</c>.
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

            if (!IsFileAllowed(uri))
                return null;
            return File.Exists(path) ? path : null;
        }

        /// <summary>
        /// determins if the found embedded file might be mapped and provided.
        /// </summary>
        /// <param name="fullPath">Full path to the file</param>
        /// <returns><c>true</c> if the file is allowed; otherwise <c>false</c>.</returns>
        protected virtual bool IsFileAllowed(string fullPath)
        {
            if (fullPath == null) throw new ArgumentNullException("fullPath");

            var extension = fullPath.Substring(fullPath.LastIndexOf('.') + 1);
            return _allowedFileExtensions.Any(x => x == extension);
        }

        /// <summary>
        /// Set extensions that are allowed to be scanned.
        /// </summary>
        /// <param name="fileExtensions">File extensions without the dot.</param>
        public void SetAllowedExtensions(IEnumerable<string> fileExtensions)
        {
            _allowedFileExtensions = fileExtensions;
        }

        #endregion
    }
}