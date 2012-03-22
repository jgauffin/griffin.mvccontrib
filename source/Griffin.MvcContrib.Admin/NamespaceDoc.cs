using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Griffin.MvcContrib
{
    /// <summary>
    /// Administration area for Griffin.MvcContrib
    /// </summary>
    /// <remarks>
    /// <para>
    /// The administration areas is currently used only for managing the localization features (the
    /// user management is not completed yet).
    /// </para>
    /// <para>
    /// Installation guidelines:
    /// <list type="table">
    /// <item>
    /// <term>Install packages</term>
    /// <description>You have probably already installed the nuget packages. If not, I suggest that you use
    /// the nuget packages instead of a manual installation. Remember to include the SqlServer or RavenDb package
    /// unless you are useing flatfiles (JSON) for the localization</description>
    /// </item>
    /// <item>
    /// <term>Configure localization</term>
    /// <description>You need to configure your inversion of control container to inject the localization providers.
    /// This step is described in the Griffin.MvcContrib.Localization namespace of the documentation for Griffin.MvcContrib base package.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Configure the administation area</term>
    /// <description>
    /// You need to use a VirtualPathProvider which can provide views from embedded resources (and modify them to include @inherits and correct @using statements). Griffin.MvcContrib
    /// contains one which you can use. Add the followning to your global.asax:
    /// <code>
    /// 
    /// </code>
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    [CompilerGenerated]
    class NamespaceDoc
    {
    }
}
