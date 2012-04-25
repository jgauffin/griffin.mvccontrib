using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Web.Hosting;
using Griffin.MvcContrib.Logging;

namespace Griffin.MvcContrib.VirtualPathProvider
{
    /// <summary>
    ///   Virtual path provider used to provide resources for Griffin framework
    /// </summary>
    /// <remarks>
    /// <para>
    /// You can easily use embedded views (using class libraries for Controllers & Views) by configuration MVC like this:
    /// <example>
    /// <code>
    /// // you can assign a custom WebViewPage or a custom layout.
    /// var fixer = new EmbeddedViewFixer(); 
    /// var provider = new EmbeddedViewFileProvider(fixer);
    /// 
    /// // Specify the assenmbly to use and the root namespace of your class library
    /// provider.Add(new NamespaceMapping(typeof (Areas.Griffin.GriffinAreaRegistration).Assembly, "Griffin.MvcContrib"));
    /// 
    /// // register the embedded view provider. You can also point on the view files directly in the file system,
    /// // which is great during development (since you can change the views on the fly)
    /// GriffinVirtualPathProvider.Current.Add(provider);
    /// HostingEnvironment.RegisterVirtualPathProvider(GriffinVirtualPathProvider.Current);
    /// </code>
    /// </example>
    /// </para>
    /// </remarks>
    /// <seealso cref="IExternalViewFixer" />
    public class GriffinVirtualPathProvider : System.Web.Hosting.VirtualPathProvider
    {
        private static readonly GriffinVirtualPathProvider Instance = new GriffinVirtualPathProvider();
        private readonly List<IViewFileProvider> _fileProviders = new List<IViewFileProvider>();
        private readonly ILogger _logger = LogProvider.Current.GetLogger<GriffinVirtualPathProvider>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="GriffinVirtualPathProvider" /> class.
        /// </summary>
        private GriffinVirtualPathProvider()
        {
        }

        /// <summary>
        ///   Gets singleton
        /// </summary>
        public static GriffinVirtualPathProvider Current
        {
            get { return Instance; }
        }

        /// <summary>
        ///   Add a new file provider
        /// </summary>
        /// <param name="fileProvider"> </param>
        public void Add(IViewFileProvider fileProvider)
        {
            if (fileProvider == null) throw new ArgumentNullException("fileProvider");

            _logger.Debug("Added new provider: " + fileProvider);
            _fileProviders.Add(fileProvider);
        }


        /// <summary>
        ///   Gets a value that indicates whether a file exists in the virtual file system.
        /// </summary>
        /// <param name="virtualPath"> The path to the virtual file. </param>
        /// <returns> true if the file exists in the virtual file system; otherwise, false. </returns>
        public override bool FileExists(string virtualPath)
        {
            return _fileProviders.Any(provider => provider.FileExists(virtualPath)) || base.FileExists(virtualPath);
        }


        /// <summary>
        ///   Creates a cache dependency based on the specified virtual paths.
        /// </summary>
        /// <param name="virtualPath"> The path to the primary virtual resource. </param>
        /// <param name="virtualPathDependencies"> An array of paths to other resources required by the primary virtual resource. </param>
        /// <param name="utcStart"> The UTC time at which the virtual resources were read. </param>
        /// <returns> A <see cref="T:System.Web.Caching.CacheDependency" /> object for the specified virtual resources. </returns>
        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies,
                                                           DateTime utcStart)
        {
            foreach (var provider in _fileProviders)
            {
                var result = provider.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
                if (result is NoCache)
                    return null;
                if (result != null)
                    return result;
            }

            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        /// <summary>
        ///   Returns a cache key to use for the specified virtual path.
        /// </summary>
        /// <param name="virtualPath"> The path to the virtual resource. </param>
        /// <returns> A cache key for the specified virtual resource. </returns>
        public override string GetCacheKey(string virtualPath)
        {
            foreach (
                var result in
                    _fileProviders.Select(provider => provider.GetCacheKey(virtualPath)).Where(result => result != null)
                )
            {
                return result;
            }

            return base.GetCacheKey(virtualPath);
        }

        /// <summary>
        ///   Gets a virtual file from the virtual file system.
        /// </summary>
        /// <param name="virtualPath"> The path to the virtual file. </param>
        /// <returns> A descendent of the <see cref="T:System.Web.Hosting.VirtualFile" /> class that represents a file in the virtual file system. </returns>
        public override VirtualFile GetFile(string virtualPath)
        {
            foreach (
                var result in
                    _fileProviders.Select(provider => provider.GetFile(virtualPath)).Where(result => result != null))
            {
                return result;
            }

            return base.GetFile(virtualPath);
        }

        /// <summary>
        ///   Returns a hash of the specified virtual paths.
        /// </summary>
        /// <param name="virtualPath"> The path to the primary virtual resource. </param>
        /// <param name="virtualPathDependencies"> An array of paths to other virtual resources required by the primary virtual resource. </param>
        /// <returns> A hash of the specified virtual paths. </returns>
        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            foreach (
                var result in
                    _fileProviders.Select(provider => provider.GetFileHash(virtualPath, virtualPathDependencies)).Where(
                        result => result != null))
            {
                return result;
            }

            return base.GetFileHash(virtualPath, virtualPathDependencies);
        }
    }
}