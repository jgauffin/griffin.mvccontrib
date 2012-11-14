using System.Diagnostics;
using System.IO;
using System.Web.Mvc;

namespace Griffin.MvcContrib.VirtualPathProvider
{
    /// <summary>
    ///   Locates views that are embedded resources.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Requires that a <see cref="IExternalViewFixer" /> is registered in your container if you want your views to look the same even if they are located in other projects.
    /// </para>
    /// <para>Each mapping should be done to the root namespace of each assembly</para>
    /// <para>AllowedFileExtensions is modified to: "cshtml", "aspx" and "ascx". Feel free to change it according to your needs.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var provider = new EmbeddedViewFileProvider(fixer);
    /// provider.Add(new NamespaceMapping(typeof (Areas.Griffin.GriffinAreaRegistration).Assembly, "Griffin.MvcContrib"));
    /// 
    /// GriffinVirtualPathProvider.Current.Add(provider);
    /// HostingEnvironment.RegisterVirtualPathProvider(GriffinVirtualPathProvider.Current);
    /// </code>
    /// </example>
    public class EmbeddedViewFileProvider : EmbeddedFileProvider
    {
        private readonly IExternalViewFixer _viewFixer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedViewFileProvider"/> class.
        /// </summary>
        /// <param name="siteRoot">Root directory of the web site</param>
        /// <example>
        /// <code>
        /// var embeddedProvider = new EmbeddedViewFileProvider(VirtualPathUtility.ToAbsolute("~/"));
        /// </code>
        /// </example>
        public EmbeddedViewFileProvider(string siteRoot) : base(siteRoot)
        {
            _viewFixer = DependencyResolver.Current.GetService<IExternalViewFixer>();
            AllowedFileExtensions = new[] {"cshtml", "ascx", "aspx"};
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedViewFileProvider"/> class.
        /// </summary>
        /// <param name="siteRoot">Root directory of the web site</param>
        /// <param name="viewFixer">View fixer</param>
        /// <example>
        /// <code>
        /// var embeddedProvider = new EmbeddedViewFileProvider(VirtualPathUtility.ToAbsolute("~/"), new ExternalViewFixer());
        /// </code>
        /// </example>
        public EmbeddedViewFileProvider(string siteRoot, IExternalViewFixer viewFixer)
            : base(siteRoot)
        {
            _viewFixer = viewFixer;
            AllowedFileExtensions = new[] {"cshtml", "ascx", "aspx"};
        }

        private Stream CorrectView(string virtualPath, Stream stream)
        {
            if (_viewFixer == null)
                return stream;

            var outStream = _viewFixer.CorrectView(virtualPath, stream);
            stream.Close();
            return outStream;
        }

        /// <summary>
        /// Resource to load. Will correct the returned views (so that they work as regular non-embedded views)
        /// </summary>
        /// <param name="virtualPath">Requested virtual path</param>
        /// <param name="resource">Identified resource (i.e. the one to load)</param>
        /// <returns>
        /// Stream that can be returned to the Virtual Path Provider.
        /// </returns>
        protected override Stream LoadStream(string virtualPath, MappedResource resource)
        {
            var stream = base.LoadStream(virtualPath, resource);

            // embedded views need a @inherits instruction
            if (stream != null && resource.FullResourceName.EndsWith(".cshtml"))
            {
                stream = CorrectView(virtualPath, stream);
            }

            return stream;
        }
    }
}