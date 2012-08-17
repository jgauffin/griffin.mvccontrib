using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Griffin.MvcContrib.VirtualPathProvider
{
    /// <summary>
    /// Used to provide embedded files (other than views).
    /// </summary>
    /// <remarks>Can be used to provide embedded content files such as images, scripts etc.</remarks>
    /// <seealso cref="EmbeddedViewFileProvider"/>
    public class EmbeddedFileProvider : IViewFileProvider
    {
        private readonly List<MappedResource> _resourceNames = new List<MappedResource>();
        private readonly string _siteRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedFileProvider"/> class.
        /// </summary>
        /// <param name="siteRoot">Root directory of the web site</param>
        /// <example>
        /// <code>
        /// var embeddedProvider = new EmbeddedFileProvider(VirtualPathUtility.ToAbsolute("~/"));
        /// </code>
        /// </example>
        public EmbeddedFileProvider(string siteRoot)
        {
            _siteRoot = siteRoot;
            AllowedFileExtensions = new[] {"png", "jpg", "jpeg", "gif", "css", "coffee", "js"};
        }

        #region IViewFileProvider Members

        /// <summary>
        ///   Checks if a file exits
        /// </summary>
        /// <param name="virtualPath"> Virtual path like "~/Views/Home/Index.cshtml" </param>
        /// <returns> <c>true</c> if found; otherwise <c>false</c> . </returns>
        public bool FileExists(string virtualPath)
        {
            var path = GetResource(virtualPath);
            return path != null;
        }

        /// <summary>
        ///   Creates a cache dependency based on the specified virtual paths
        /// </summary>
        /// <param name="virtualPath"> Virtual path like "~/Views/Home/Index.cshtml" </param>
        /// <param name="dependencies"> The dependencies. </param>
        /// <param name="utcStart"> The UTC start. </param>
        /// <returns> A CacheDependency if the file is found and caching should be used; <see cref="NoCache.Instance" /> if caching should be disabled for the file; <c>null</c> if file is not found. </returns>
        public CacheDependency GetCacheDependency(string virtualPath, IEnumerable dependencies, DateTime utcStart)
        {
            return GetResource(virtualPath) != null ? NoCache.Instance : null;
        }

        /// <summary>
        /// </summary>
        /// <param name="virtualPath"> Virtual path like "~/Views/Home/Index.cshtml" </param>
        /// <returns> CacheDependency if found; otherwise <c>false</c> . </returns>
        public string GetCacheKey(string virtualPath)
        {
            var resource = GetResource(virtualPath);
            return resource != null ? resource.FullResourceName : null;
        }

        /// <summary>
        ///   Get file hash.
        /// </summary>
        /// <param name="virtualPath"> Virtual path like "~/Views/Home/Index.cshtml" </param>
        /// <param name="dependencies"> The dependencies. </param>
        /// <returns> a new hash each time the file have changed (if file is found); otherwise null </returns>
        public string GetFileHash(string virtualPath, IEnumerable dependencies)
        {
            return null;
        }

        /// <summary>
        ///   Get the view
        /// </summary>
        /// <param name="virtualPath"> Virtual path like "~/Views/Home/Index.cshtml" </param>
        /// <returns> File </returns>
        public virtual VirtualFile GetFile(string virtualPath)
        {
            var resource = GetResource(virtualPath);
            if (resource == null)
                return null;
            
            var stream = LoadStream(virtualPath, resource);
            return stream == null ? null : new EmbeddedFile(virtualPath, stream);
        }

        #endregion

        /// <summary>
        /// Resource to load
        /// </summary>
        /// <param name="virtualPath">Requested virtual path</param>
        /// <param name="resource">Identified resource (i.e. the one to load)</param>
        /// <returns>Stream that can be returned to the Virtual Path Provider.</returns>
        /// <remarks>The default implementation uses <c>resource.Assembly.GetManifestResourceStream(resource.FullResourceName)</c></remarks>
        protected virtual Stream LoadStream(string virtualPath, MappedResource resource)
        {
            if (resource == null) throw new ArgumentNullException("resource");
            var stream = resource.Assembly.GetManifestResourceStream(resource.FullResourceName);
            return stream;
        }

        /// <summary>
        ///   Add a namespace mapping for embedded resources.
        /// </summary>
        /// <param name="mapping"> Mapping to add </param>
        public virtual void Add(NamespaceMapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException("mapping");

            Map(mapping);
        }


        /// <summary>
        ///   Get resource name by scanning all mapped namespaces.
        /// </summary>
        /// <param name="uri"> Uri to search for. </param>
        /// <returns> Full resource name if found; otherwise null. </returns>
        private MappedResource GetResource(string uri)
        {
            if (uri.StartsWith("~"))
                uri = VirtualPathUtility.ToAbsolute(uri);
            if (uri.StartsWith(_siteRoot))
                uri = uri.Remove(0, _siteRoot.Length);

            uri = uri.TrimStart('/').TrimEnd('/');
            uri = uri.Replace('/', '.');
            var result =
                _resourceNames.FirstOrDefault(
                    resource => resource.ResourceName.Equals(uri, StringComparison.OrdinalIgnoreCase));
            return result;
        }

        /// <summary>
        /// Create mappings for all resources in a specific namespace (and all sub namespaces).
        /// </summary>
        /// <param name="mapping">Mapping to load embedded resources in</param>
        protected void Map(NamespaceMapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException("mapping");

            var names = mapping.Assembly.GetManifestResourceNames();
            foreach (var name in names)
            {
                if (!name.StartsWith(mapping.FolderNamespace))
                    continue;
                
                if (!IsFileAllowed(name))
                    continue;

                _resourceNames.Add(new MappedResource
                                       {
                                           Assembly = mapping.Assembly,
                                           AssemblyDate = new FileInfo(mapping.Assembly.Location).CreationTimeUtc,
                                           FullResourceName = name,
                                           ResourceRoot = mapping.FolderNamespace,
                                           ResourceName = name.Remove(0, mapping.FolderNamespace.Length + 1)
                                           // include the last dot
                                       });
            }
        }

        /// <summary>
        /// determins if the found embedded file might be mapped and provided.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        /// <remarks>Default implementation uses <see cref="AllowedFileExtensions"/> to determine which files to servce.</remarks>
        protected virtual bool IsFileAllowed(string resourceName)
        {
            if (resourceName == null) throw new ArgumentNullException("resourceName");

            var extension = resourceName.Substring(resourceName.LastIndexOf('.') + 1);
            return AllowedFileExtensions.Any(x => x == extension);
        }

        /// <summary>
        /// Gets or sets file extensions that may be served.
        /// </summary>
        /// <remarks>Default extensions: <code>new[] {"png", "jpg", "jpeg", "gif", "css", "coffee", "js"}</code></remarks>
        public string[] AllowedFileExtensions { get; set; }


        #region Nested type: EmbeddedFile

        protected class EmbeddedFile : VirtualFile
        {
            private readonly Stream _resourceStream;

            public EmbeddedFile(string virtualPath, Stream resourceStream)
                : base(virtualPath)
            {
                _resourceStream = resourceStream;
            }

            /// <summary>
            ///   When overridden in a derived class, returns a read-only stream to the virtual resource.
            /// </summary>
            /// <returns> A read-only stream to the virtual file. </returns>
            public override Stream Open()
            {
                return _resourceStream;
            }
        }

        #endregion

        #region Nested type: MappedResource

        protected class MappedResource
        {
            public string ResourceRoot { get; set; }
            public string FullResourceName { get; set; }
            public string ResourceName { get; set; }

            public Assembly Assembly { get; set; }
            public DateTime AssemblyDate { get; set; }

            public override string ToString()
            {
                return ResourceName + "/" + FullResourceName;
            }
        }

        #endregion
    }
}