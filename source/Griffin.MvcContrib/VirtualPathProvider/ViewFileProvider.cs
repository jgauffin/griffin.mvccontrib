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
    /// <para>
    /// Using this provider lets you keep your view files in any folder. Combining this provider with the embedded provider is a great
    /// way to be able to change views during development (using the file provider) and then include the views from the DLL in production.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var provider = new DiskFileLocator(new DiskProvider("/MyArea/", @"..\..\MyClassLibrary\Areas\MyArea\Views"));
    /// GriffinVirtualPathProvider.Current.Add(provider);
    /// HostingEnvironment.RegisterVirtualPathProvider(GriffinVirtualPathProvider.Current);
    /// </code>
    /// </example>
    /// <seealso cref="IViewFileLocator"/>
    public class ViewFileProvider : IViewFileProvider
    {
        private readonly IViewFileLocator _viewFileLocator;
        private readonly IExternalViewFixer _viewFixer;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ViewFileProvider" /> class.
        /// </summary>
        /// <param name="viewFileLocator"> The view file locator. </param>
        public ViewFileProvider(IViewFileLocator viewFileLocator)
        {
            if (viewFileLocator == null) throw new ArgumentNullException("viewFileLocator");
            _viewFileLocator = viewFileLocator;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ViewFileProvider" /> class.
        /// </summary>
        /// <param name="viewFileLocator"> The view file locator. </param>
        /// <param name="viewFixer">Used to modify external views so that can be written as any other view.</param>
        public ViewFileProvider(IViewFileLocator viewFileLocator, IExternalViewFixer viewFixer)
        {
            if (viewFileLocator == null) throw new ArgumentNullException("viewFileLocator");
            _viewFileLocator = viewFileLocator;
            _viewFixer = viewFixer;
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

        /// <summary>
        /// Returns a cache key to use for the specified virtual path
        /// </summary>
        /// <param name="virtualPath">Virtual path like "~/Views/Home/Index.cshtml"</param>
        /// <returns>CacheDependency if found; otherwise <c>false</c>.</returns>
        public string GetCacheKey(string virtualPath)
        {
            return null;
        }

        #endregion

        /// <summary>
        /// Used to adjust the external views before they are returned
        /// </summary>
        /// <param name="virtualPath">Path to requested view</param>
        /// <param name="fileStream">Loaded file</param>
        /// <returns>Stream to use</returns>
        protected virtual Stream CorrectView(string virtualPath, FileStream fileStream)
        {
            var fixer = _viewFixer ?? DependencyResolver.Current.GetService<IExternalViewFixer>();
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