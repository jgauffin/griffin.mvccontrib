using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Griffin.MvcContrib.VirtualPathProvider
{
    /// <summary>
    /// The virtual path provider can be used as an alternative to the one which comes with the .NET framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The virtual path provider is named <see cref="GriffinVirtualPathProvider"/>. Look at it's documentation for further information.
    /// </para>
    /// <para>
    /// To be able to serve static files you have to tell IIS that it should pass those to the virtual path provider. It's done in web.config
    /// like this:
    /// <example>
    ///  <system.webServer>
    ///     <modules runAllManagedModulesForAllRequests="true" />
    ///     <handlers>
    ///       <add name="AspNetStaticFileHandler-GIF" path="*.gif" verb="GET,HEAD" type="System.Web.StaticFileHandler"/>
    ///       <add name="AspNetStaticFileHandler-PNG" path="*.png" verb="GET,HEAD" type="System.Web.StaticFileHandler"/>
    ///       <add name="AspNetStaticFileHandler-JPG" path="*.jpg" verb="GET,HEAD" type="System.Web.StaticFileHandler"/>
    ///       <add name="AspNetStaticFileHandler-CSS" path="*.css" verb="GET,HEAD" type="System.Web.StaticFileHandler"/>
    ///       <add name="AspNetStaticFileHandler-JS" path="*.js" verb="GET,HEAD" type="System.Web.StaticFileHandler"/>
    ///     </handlers>
    ///   </system.webServer>
    /// </example>
    /// You also probably do not want the static files to go through the MVC routing, so add the following ignore in global.asax:
    /// <example>
    /// <code>
    /// routes.IgnoreRoute("{*staticfile}", new { staticfile = @".*\.(css|js|gif|jpg|png)(/.*)?" });
    /// </code>
    /// </example>
    /// </para>
    /// </remarks>
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}
