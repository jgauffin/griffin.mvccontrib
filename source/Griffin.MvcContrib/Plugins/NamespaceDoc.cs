using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Griffin.MvcContrib.VirtualPathProvider;

namespace Griffin.MvcContrib.Plugins
{
    /// <summary>
    /// This framework can be used to create a plugin system for ASP.NET MVC3 together with an inversion of control container.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Convience over configuration framework for plugins. You have to use a folder structure similar to:
    /// <code>
    /// ProjectName\Plugins\
    /// ProjectName\Plugins\PluginName
    /// ProjectName\Plugins\PluginName\Plugin.PluginName // the plugin project
    /// ProjectName\Plugins\PluginName\Plugin.PluginName.Tests
    /// ProjectName\ProjectName.Mvc3 // The MVC project
    /// </code>
    /// 
    /// </para>
    /// <para>
    /// The <see cref="PluginFinder"/> will help you load plugin DLL:s from a plugin folder. Use <see cref="PluginFinder.Assemblies"/> to
    /// register all controllers in your favorite IoC container. The use <see cref="GriffinVirtualPathProvider"/> to load your plugin views
    /// from disk or embedded resources. The <see cref="ExternalViewFixer"/> allows you to keep the views without any code changes (which is 
    /// required otherwise for views in class libraries).
    /// </para>
    /// <para>
    /// You can use <see cref="PluginFileLocator"/> to be able to edit the views at runtime in Visual Studio.
    /// </para>
    /// </remarks>
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}
