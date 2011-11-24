using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Caching;
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

    public class NamespaceMapping
    {
        private readonly Assembly _assembly;
        private readonly string _folderNamespace;

        public NamespaceMapping(Assembly assembly, string folderNamespace)
        {
            _assembly = assembly;
            _folderNamespace = folderNamespace;
        }

        public Assembly Assembly
        {
            get { return _assembly; }
        }

        public string FolderNamespace
        {
            get { return _folderNamespace; }
        }
    }

    /// <summary>
    /// Virtual path provider used to provide resources for Griffin framework
    /// </summary>
    public class GriffinVirtualPathProvider : VirtualPathProvider
    {
        private readonly List<MappedNamespace> _namespaces = new List<MappedNamespace>();
        private readonly List<MappedResource> _resourceNames = new List<MappedResource>();
        
        private class MappedNamespace
        {
            public Assembly Assembly { get; set; }
            public string Namespace { get; set; }
        }

        private class MappedResource
        {
            public string ResourceRoot { get; set; }
            public string FullResourceName { get; set; }
            public string ResourceName { get; set; }

            public Assembly Assembly { get; set; }
            public DateTime AssemblyDate { get; set; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GriffinVirtualPathProvider"/> class.
        /// </summary>
        public GriffinVirtualPathProvider(params NamespaceMapping[] mappings)
        {
            foreach (var mapping in mappings)
            {
                Map(mapping);
            }
        }

        private void Map(NamespaceMapping mapping)
        {
            var names = mapping.Assembly.GetManifestResourceNames();
            foreach (var name in names)
            {
                if (!name.StartsWith(mapping.FolderNamespace))
                    continue;


                _resourceNames.Add(new MappedResource
                                    {
                                        Assembly = mapping.Assembly,
                                        AssemblyDate = new FileInfo(mapping.Assembly.Location).CreationTimeUtc,
                                        FullResourceName = name,
                                        ResourceRoot = mapping.FolderNamespace,
                                        ResourceName = name.Remove(0, mapping.FolderNamespace.Length + 1) // include the last dot
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
            var resource = GetResource(virtualPath);
            return resource != null ? null : base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        /// <summary>
        /// Get resource name by scanning all mapped namespaces.
        /// </summary>
        /// <param name="uri">Uri to search for.</param>
        /// <returns>Full resource name if found; otherwise null.</returns>
        private MappedResource GetResource(string uri)
        {
            uri = uri.TrimStart('~', '/').TrimEnd('/');
            uri=uri.Replace('/', '.');

            var result= _resourceNames.Where(resource => resource.ResourceName.Equals(uri, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (result != null)
                Debug.WriteLine("Exists: " + uri);

            return result;
        }

        /// <summary>
        /// Returns a cache key to use for the specified virtual path.
        /// </summary>
        /// <param name="virtualPath">The path to the virtual resource.</param>
        /// <returns>
        /// A cache key for the specified virtual resource.
        /// </returns>
        public override string GetCacheKey(string virtualPath)
        {
            var resource = GetResource(virtualPath);
            return resource != null ? resource.FullResourceName : base.GetCacheKey(virtualPath);
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
            return GetResource(virtualPath) != null || base.FileExists(virtualPath);
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
            var resource = GetResource(virtualPath);
            if (resource != null)
            {
                Debug.WriteLine("Returning : " + virtualPath);
                var stream = resource.Assembly.GetManifestResourceStream(resource.FullResourceName);

				// embedded views need a @inherits instruction
                if (stream != null && resource.FullResourceName.StartsWith("Griffin") && resource.FullResourceName.EndsWith(".cshtml"))
                	stream = CorrectEmbeddedView(virtualPath, stream);

            	return stream == null ? base.GetFile(virtualPath) : new EmbeddedFile(virtualPath, stream);
            }

            return base.GetFile(virtualPath);
        }

    	private static MemoryStream CorrectEmbeddedView(string virtualPath, Stream stream)
    	{
    		var reader = new StreamReader(stream);
    		var view = reader.ReadToEnd();
    		stream.Close();
    		var ourStream = new MemoryStream();
    		var writer = new StreamWriter(ourStream);

    		string modelString = "";
    		var modelPos = view.IndexOf("@model");
    		if (modelPos != -1)
    		{
    			writer.Write(view.Substring(0, modelPos));
    			int modelEndPos = view.IndexOfAny(new char[] {'\r', '\n'}, modelPos);
    			modelString = view.Substring(modelPos, modelEndPos - modelPos);
    			view = view.Remove(0, modelEndPos);
    		}

    		writer.WriteLine("@using System.Web.Mvc");
    		writer.WriteLine("@using System.Web.Mvc.Ajax");
    		writer.WriteLine("@using System.Web.Mvc.Html");
    		writer.WriteLine("@using System.Web.Routing");
    		writer.WriteLine("@using Griffin.MvcContrib");

    		if (virtualPath.ToLower().Contains("__viewstart"))
    			writer.WriteLine("@inherits System.Web.WebPages.StartPage");
    		else if (modelString == "@model object")
    			writer.WriteLine("@inherits System.Web.Mvc.WebViewPage<dynamic>");
    		else if (!string.IsNullOrEmpty(modelString))
    			writer.WriteLine("@inherits System.Web.Mvc.WebViewPage<" + modelString.Substring(7) + ">");
    		else
    			writer.WriteLine("@inherits System.Web.Mvc.WebViewPage");

    		writer.WriteLine("@{ Layout = \"~/Views/Shared/_Layout.cshtml\"; }");
    		writer.Write(view);
    		writer.Flush();
    		ourStream.Position = 0;
    		return ourStream;
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
