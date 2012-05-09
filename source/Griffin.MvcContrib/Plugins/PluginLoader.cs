using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using Griffin.MvcContrib.Logging;

namespace Griffin.MvcContrib.Plugins
{
    /// <summary>
    ///   Loads plugin assemblies
    /// </summary>
    /// <remarks>
    /// <para>This class needs to be run before Application_Start is executed. The typical approach is to load it
    /// from a class which is invoked by the <see cref="PreApplicationStartMethodAttribute" />.
    /// </para>
    /// <para>
    ///   Read http://haacked.com/archive/2010/05/16/three-hidden-extensibility-gems-in-asp-net-4.aspx
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// [assembly: PreApplicationStartMethod(typeof (YourApp.UI.PluginProvider), "LoadPlugins")]
    /// namespace YourApp.UI
    /// {
    ///     // Note that the PluginLoader is a singleton
    ///     // 
    ///     public static class PluginProvider
    ///     {
    ///         public static PluginLoader Loader;
    ///    
    ///         public static void LoadPlugins()
    ///         {
    ///             var path = Directory.Exists(HostingEnvironment.MapPath("~/bin/Plugins/"))
    ///                            ? HostingEnvironment.MapPath("~/bin/Plugins/")
    ///                            : HostingEnvironment.MapPath("~/Plugins/");
    ///
    ///
    ///             Loader = new PluginLoader(path);
    ///         }
    ///     }    
    /// }
    /// </code>
    /// </example>
    public class PluginLoader
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly ILogger _logger = LogProvider.Current.GetLogger<PluginLoader>();
        private readonly DirectoryInfo _pluginFolder;

        /// <summary>
        ///   Initializes the <see cref="PluginLoader" /> class.
        /// </summary>
        /// <param name="virtualPluginFolderPath"> App relative path to plugin folder </param>
        /// <example>
        ///   <code>var loader = new PluginLoader("~/"); // all plugins are located in the root folder.</code>
        /// </example>
        public PluginLoader(string virtualPluginFolderPath)
        {
            if (virtualPluginFolderPath == null) throw new ArgumentNullException("virtualPluginFolderPath");
            var path = virtualPluginFolderPath.StartsWith("~")
                           ? HostingEnvironment.MapPath(virtualPluginFolderPath)
                           : virtualPluginFolderPath;
            if (path == null)
                throw new InvalidOperationException(string.Format("Failed to map path '{0}'.", virtualPluginFolderPath));

            _pluginFolder = new DirectoryInfo(path);
            Startup();
        }

        /// <summary>
        ///   Get all plugin assemblies.
        /// </summary>
        public IEnumerable<Assembly> Assemblies
        {
            get { return _assemblies; }
        }

        /// <summary>
        ///   Called during startup to scan for all plugin assemblies
        /// </summary>
        public void Startup()
        {
            CopyPluginDlls(_pluginFolder, AppDomain.CurrentDomain.DynamicDirectory);
        }

        private void CopyPluginDlls(DirectoryInfo sourceFolder, string destinationFolder)
        {
            foreach (var plug in sourceFolder.GetFiles("*.dll", SearchOption.AllDirectories))
            {
                if (!File.Exists(Path.Combine(destinationFolder, plug.Name)))
                {
                    File.Copy(plug.FullName, Path.Combine(destinationFolder, plug.Name), false);
                }
                LoadPluginAssembly(plug.FullName);
            }
        }

        private void LoadPluginAssembly(string fullPath)
        {
            if (fullPath == null) throw new ArgumentNullException("fullPath");

            try
            {
                var assembly = Assembly.Load(AssemblyName.GetAssemblyName(fullPath));
                BuildManager.AddReferencedAssembly(assembly);
                _assemblies.Add(assembly);
            }
            catch (Exception err)
            {
                _logger.Warning("Failed to load " + fullPath + ".", err);

                var loaderEx = err as ReflectionTypeLoadException;
                if (loaderEx != null)
                {
                    foreach (var exception in loaderEx.LoaderExceptions)
                    {
                        _logger.Warning(string.Format("Loader exception for file '{0}'.", fullPath), exception);
                    }
                }

                throw;
            }
        }
    }
}