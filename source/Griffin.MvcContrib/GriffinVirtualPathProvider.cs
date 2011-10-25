using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;

namespace Griffin.MvcContrib
{
	public class MyViewEngine : System.Web.Mvc.RazorViewEngine
	{
		public MyViewEngine()
		//  : base()
		{
			this.ViewLocationFormats = new string[]
            {
                //{0} - Culture Name, {1} - Controller, {2} - Page, {3} Extension (aspx/ascx)
                "~/Views/{0}/{1}",
                "~/Views/en-US/{1}/{2}{3}",
                "~/Views/Shared/{1}/{2}{3}",
                "~/Views/Shared/{2}{3}"/**/
            };
		}

		public override System.Web.Mvc.ViewEngineResult FindView(System.Web.Mvc.ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			return FindView(controllerContext, viewName, masterName, useCache, ".cshtml");
		}

		private System.Web.Mvc.ViewEngineResult FindView(System.Web.Mvc.ControllerContext controllerContext, string viewName, string masterName, bool useCache, string extension)
		{
			if (controllerContext.RequestContext.HttpContext.Request.Url.AbsolutePath.StartsWith("/localization/"))
			{
				return new System.Web.Mvc.ViewEngineResult(CreateView(controllerContext, controllerContext.RequestContext.HttpContext.Request.Url.AbsolutePath + extension, masterName), this);
			}

			return base.FindView(controllerContext, viewName, masterName, useCache);
		}
	}

    /// <summary>
    /// Virtual path provider used to provide resources for Griffin framework
    /// </summary>
    public class GriffinVirtualPathProvider : VirtualPathProvider
    {
        private readonly string[] _resourceRoots = new string[] {"Griffin.MvcContrib.Areas"};
		private readonly List<MappedResource> _resourceNames = new List<MappedResource>();

		private class MappedResource
		{
			public string ResourceRoot { get; set; }
			public string FullResourceName { get; set; }
			public string ResourceName { get; set; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GriffinVirtualPathProvider"/> class.
		/// </summary>
        public GriffinVirtualPathProvider()
        {
            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (var name in names)
            {
            	var resourceRoot = _resourceRoots.First(root => name.StartsWith(root));
                if (resourceRoot == null)
					continue;

                
					_resourceNames.Add(new MappedResource
					                   	{
					                   		FullResourceName = name,
											ResourceRoot = resourceRoot,
											ResourceName = name.Remove(0, resourceRoot.Length+1) // include the last dot
					                   	});
                	
                
            }
        }

		/// <summary>
		/// Creates a cache dependency based on the specified virtual paths.
		/// </summary>
		/// <param name="virtualPath">The path to the primary virtual resource.</param>
		/// <param name="virtualPathDependencies">An array of paths to other resources required by the primary virtual resource.</param>
		/// <param name="utcStart">The UTC time at which the virtual resources were read.</param>
		/// <returns>
		/// A <see cref="T:System.Web.Caching.CacheDependency"/> object for the specified virtual resources.
		/// </returns>
		public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
		{
			if (GetResourceName(virtualPath) != null)
				return null;

			return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
		}

		/// <summary>
		/// Get resource name by scanning all mapped namespaces.
		/// </summary>
		/// <param name="uri">Uri to search for.</param>
		/// <returns>Full resource name if found; otherwise null.</returns>
        protected string GetResourceName(string uri)
		{
			uri = uri.TrimStart('~', '/').TrimEnd('/');
            uri=uri.Replace('/', '.');

            var result = _resourceNames.Where(resource => resource.ResourceName.Equals(uri, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
			return result != null ? result.FullResourceName : null;
		}

		public override string GetCacheKey(string virtualPath)
		{
			return null;
		}

		/// <summary>
		/// Gets a value that indicates whether a file exists in the virtual file system.
		/// </summary>
		/// <param name="virtualPath">The path to the virtual file.</param>
		/// <returns>
		/// true if the file exists in the virtual file system; otherwise, false.
		/// </returns>
        public override bool FileExists(string virtualPath)
        {
            return GetResourceName(virtualPath) != null || base.FileExists(virtualPath);
        }

		/// <summary>
		/// Gets a virtual file from the virtual file system.
		/// </summary>
		/// <param name="virtualPath">The path to the virtual file.</param>
		/// <returns>
		/// A descendent of the <see cref="T:System.Web.Hosting.VirtualFile"/> class that represents a file in the virtual file system.
		/// </returns>
        public override VirtualFile GetFile(string virtualPath)
        {
			Debug.WriteLine("Returning : " + virtualPath);
        	var resourceName = GetResourceName(virtualPath);
			if (resourceName != null)
			{
				var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
				return stream == null ? base.GetFile(virtualPath) : new EmbeddedFile(virtualPath, stream);
				
			}

			return base.GetFile(virtualPath);
        }

        class EmbeddedFile : VirtualFile
        {
            private readonly Stream _resourceStream;

            public EmbeddedFile(string virtualPath, Stream resourceStream) : base(virtualPath)
            {
                _resourceStream = resourceStream;
            }

            /// <summary>
            /// When overridden in a derived class, returns a read-only stream to the virtual resource.
            /// </summary>
            /// <returns>
            /// A read-only stream to the virtual file.
            /// </returns>
            public override Stream Open()
            {
                return _resourceStream;
            }

        }
    }
}
