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
    ///   Uses <see cref="IEmbeddedViewFixer" /> on all found views
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
        ///   Creates a cache dependency based on the specified virtual paths
        /// </summary>
        /// <param name="virtualPath"> Virtual path like "~/Views/Home/Index.cshtml" </param>
        /// <returns> CacheDependency if found; otherwise <c>false</c> . </returns>
        public CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies,
                                                  DateTime utcStart)
        {
            var fullPath = _viewFileLocator.GetFullPath(virtualPath);
            return fullPath != null ? new CacheDependency(fullPath) : null;
        }

        /// <summary>
        ///   Get file hash.
        /// </summary>
        /// <param name="virtualPath"> Virtual path like "~/Views/Home/Index.cshtml" </param>
        /// <returns> a new hash each time the file have changed (if file is found); otherwise null </returns>
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
            var fixer = DependencyResolver.Current.GetService<IEmbeddedViewFixer>();
            var fixedView = fixer.CorrectView(virtualPath, fileView);
            return new FileResource(virtualPath, fixedView);
        }

        public string GetCacheKey(string virtualPath)
        {
            return null;
        }

        #endregion

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