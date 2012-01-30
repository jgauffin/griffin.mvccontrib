using System;
using System.Collections;
using System.IO;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Griffin.MvcContrib.VirtualPathProvider
{
    /// <summary>
    ///   Provides view files from disk
    /// </summary>
    /// <remarks>
    ///   Requires that a <see cref="IEmbeddedViewFixer" /> is registered in your container if you want your views to look the same even if they are located in other projects.
    /// </remarks>
    public class ViewFileProvider : IViewFileProvider
    {
        private readonly IViewFileLocator _viewFileLocator;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ViewFileProvider" /> class.
        /// </summary>
        /// <param name="viewFileLocator"> The view file locator. </param>
        public ViewFileProvider(IViewFileLocator viewFileLocator)
        {
            if (viewFileLocator == null) throw new ArgumentNullException("viewFileLocator");
            _viewFileLocator = viewFileLocator;
        }

        #region IViewFileProvider Members

        /// <summary>
        ///   Checks if a file exists in this provider
        /// </summary>
        /// <param name="virtualPath"> Path </param>
        /// <returns> Determines if a file exists in this provider </returns>
        public bool FileExists(string virtualPath)
        {
            return _viewFileLocator.GetFullPath(virtualPath) != null;
        }

        /// <summary>
        /// Creates a cache dependency based on the specified virtual paths
        /// </summary>
        /// <param name="virtualPath">Virtual path like "~/Views/Home/Index.cshtml"</param>
        /// <param name="virtualPathDependencies">An array of paths to other resources required by the primary virtual resource</param>
        /// <param name="utcStart">The UTC time at which the virtual resources were read</param>
        /// <returns>
        /// CacheDependency if found; otherwise <c>false</c> .
        /// </returns>
        public CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies,
                                                  DateTime utcStart)
        {
            var fullPath = _viewFileLocator.GetFullPath(virtualPath);
            return fullPath != null ? new CacheDependency(fullPath) : null;
        }

        /// <summary>
        /// Get file hash.
        /// </summary>
        /// <param name="virtualPath">Virtual path like "~/Views/Home/Index.cshtml"</param>
        /// <param name="virtualPathDependencies">An array of paths to other virtual resources required by the primary virtual resource</param>
        /// <returns>
        /// a new hash each time the file have changed (if file is found); otherwise null
        /// </returns>
        public string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            var fullPath = _viewFileLocator.GetFullPath(virtualPath);
            return fullPath != null ? File.GetLastWriteTime(fullPath).ToString() : null;
        }

        /// <summary>
        ///   Get the view
        /// </summary>
        /// <param name="virtualPath"> Virtual path like "~/Views/Home/Index.cshtml" </param>
        /// <returns> File </returns>
        public VirtualFile GetFile(string virtualPath)
        {
            var fullPath = _viewFileLocator.GetFullPath(virtualPath);
            if (fullPath == null)
                return null;

            var fileView = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var fixedView = CorrectView(virtualPath, fileView);
            return new FileResource(virtualPath, fixedView);
        }

        public string GetCacheKey(string virtualPath)
        {
            return null;
        }

        #endregion

        private static Stream CorrectView(string virtualPath, FileStream fileStream)
        {
            var fixer = DependencyResolver.Current.GetService<IEmbeddedViewFixer>();
            if (fixer == null)
            {
                return fileStream;
            }

            var fixedView = fixer.CorrectView(virtualPath, fileStream);
            fileStream.Close();
            return fixedView;
        }

        #region Nested type: FileResource

        private class FileResource : VirtualFile
        {
            private readonly Stream _stream;
            private readonly string _virtualPath;

            public FileResource(string virtualPath, Stream stream)
                : base(virtualPath)
            {
                _virtualPath = virtualPath;
                _stream = stream;
            }

            public override bool IsDirectory
            {
                get { return false; }
            }

            public override Stream Open()
            {
                return _stream;
            }
        }

        #endregion
    }
}