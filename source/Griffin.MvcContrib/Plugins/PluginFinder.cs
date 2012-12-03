using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;
using Griffin.MvcContrib.Logging;

namespace Griffin.MvcContrib.Plugins
{
    /// <summary>
    /// Finds all plugins and load them into the app domain (or use previously loaded assemblies)
    /// </summary>
    /// <remarks>
    /// Loads all plugins whos file name starts with "Plugin." </remarks>
    /// <seealso cref="Griffin.MvcContrib.Plugins"/>
    public class PluginFinder
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly ILogger _logger = LogProvider.Current.GetLogger<PluginFinder>();
        private string _path;

        /// <summary>
        ///   Initializes the <see cref="PluginFinder" /> class.
        /// </summary>
        /// <param name="virtualPluginFolderPath"> App relative path to plugin folder </param>
        /// <example>
        ///   <code>var loader = new PluginLoader("~/"); // all plugins are located in the root folder.</code>
        /// </example>
        public PluginFinder(string virtualPluginFolderPath)
        {
            if (virtualPluginFolderPath == null) throw new ArgumentNullException("virtualPluginFolderPath");
            var path = virtualPluginFolderPath.StartsWith("~")
                           ? HostingEnvironment.MapPath(virtualPluginFolderPath)
                           : virtualPluginFolderPath;
            if (path == null)
                throw new InvalidOperationException(string.Format("Failed to map path '{0}'.", virtualPluginFolderPath));

            _path = path;
        }

        /// <summary>
        ///   Gets all loaded plugin assemblies.
        /// </summary>
        public IEnumerable<Assembly> Assemblies
        {
            get { return _assemblies; }
        }

        /// <summary>
        /// Called during startup to scan for all plugin assemblies
        /// </summary>
        public void Find()
        {
            Find("Plugin.*.dll");
        }

        /// <summary>
        /// Called during startup to scan for all plugin assemblies, specifing the plugin filter
        /// </summary>
        public void Find(string pluginMask)
        {
            foreach (var file in Directory.GetFiles(_path, pluginMask))
            {
                LoadPluginAssembly(file);
            }
        }

        private void LoadPluginAssembly(string fullPath)
        {
            if (fullPath == null) throw new ArgumentNullException("fullPath");

            try
            {
                var assembly = Assembly.LoadFrom(fullPath);
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