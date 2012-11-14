using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Griffin.MvcContrib.VirtualPathProvider;

namespace Griffin.MvcContrib
{
    /// <summary>
    /// Extension to make the configuration easier.
    /// </summary>
    public static class GriffinVirtualPathProviderExtensions
    {
        /// <summary>
        /// Register the content files used by the adminstration area.
        /// </summary>
        /// <param name="provider"><c>GriffinVirtualPathProvider.Current</c></param>
        /// <param name="layoutVirtualPath">Typically <c>"~/Views/Shared/_Layout.cshtml"</c></param>
        public static void RegisterAdminFiles(this GriffinVirtualPathProvider provider, string layoutVirtualPath)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            if (layoutVirtualPath == null) throw new ArgumentNullException("layoutVirtualPath");
            // you can assign a custom WebViewPage or a custom layout in EmbeddedViewFixer.
            var fixer = new ExternalViewFixer()
            {
                LayoutPath = layoutVirtualPath
            };

            var griffinAssembly = typeof(MvcContrib.Areas.Griffin.GriffinAreaRegistration).Assembly;

            // for view files
            var embeddedViews = new EmbeddedViewFileProvider(VirtualPathUtility.ToAbsolute("~/"), fixer);
            embeddedViews.Add(new NamespaceMapping(griffinAssembly, "Griffin.MvcContrib"));
            provider.Add(embeddedViews);
            
            // Add support for loading content files:
            var contentFilesProvider = new EmbeddedFileProvider(VirtualPathUtility.ToAbsolute("~/"));
            contentFilesProvider.Add(new NamespaceMapping(griffinAssembly, "Griffin.MvcContrib"));
            provider.Add(contentFilesProvider);

        }
    }
}
