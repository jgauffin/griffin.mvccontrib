using System.Runtime.CompilerServices;

namespace Griffin.MvcContrib
{
    /// <summary>
    /// Administration area for Griffin.MvcContrib
    /// </summary>
    /// <remarks>
    /// <img src="screenshot.png" />
    /// <para>
    /// The administration areas is currently used only for managing the localization features (the
    /// user management is not completed yet).
    /// 
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
    /// // disable role checking, or specify a role name
    /// GriffinAdminRoles.Translator = null;
    /// GriffinAdminRoles.HomePage = null;
    /// 
    /// // inject the localization features into MVC
    /// ModelValidatorProviders.Providers.Clear();
    /// ModelMetadataProviders.Current = new LocalizedModelMetadataProvider();
    /// ModelValidatorProviders.Providers.Add(new LocalizedModelValidatorProvider());
    /// 
    /// // add the controllers (using autofac)
    /// builder.RegisterControllers(typeof(MvcContrib.Areas.Griffin.GriffinAreaRegistration).Assembly);
    /// 
    /// <![CDATA[
    /// // Loads strings from repositories  (using autofac)
    /// builder.RegisterType<RepositoryStringProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
    /// builder.RegisterType<ViewLocalizer>().AsImplementedInterfaces().InstancePerLifetimeScope();
    /// builder.RegisterType<SqlLocalizedTypesRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
    /// builder.RegisterType<SqlLocalizedViewsRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
    /// 
    /// // Connection factory used by the SQL providers  (using autofac)
    /// builder.RegisterInstance(new AdoNetConnectionFactory("DemoDb")).AsSelf();
    /// builder.RegisterType<LocalizationDbContext>().AsImplementedInterfaces().InstancePerLifetimeScope();
    /// ]]>
    /// 
    /// // configure the content files used by the admin area.
    /// GriffinVirtualPathProvider.Current.RegisterAdminFiles("~/Views/Shared/_Layout.cshtml");
    /// HostingEnvironment.RegisterVirtualPathProvider(GriffinVirtualPathProvider.Current);
    /// 
    /// </code>
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}