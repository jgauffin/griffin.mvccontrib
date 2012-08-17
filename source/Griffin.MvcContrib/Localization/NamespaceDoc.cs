using System.Runtime.CompilerServices;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Localization.Views;

namespace Griffin.MvcContrib.Localization
{
    /// <summary>
    /// Classes making it easier to handle localization in MVC
    /// </summary>
    /// <remarks>
    /// The localization features can be activated in several levels. 
    /// <para>
    /// <list type="table">
    /// <item>
    /// <term>Translate only</term>
    /// <description>
    /// You need to setup the <see cref="LocalizedModelValidatorProvider"/> and <see cref="LocalizedModelMetadataProvider"/> in your
    /// global.asax. The easiest way to provide strings for them is to use string tables with the help of <see cref="ResourceStringProvider"/> class.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Manage translations</term>
    /// <description>
    /// <para>
    /// You can also manage translations by using the <see cref="IViewLocalizationRepository"/> and the
    /// <see cref="ILocalizedTypesRepository"/> interfaces. Register one of the implementations in your inversion of control
    /// container. Dont forget to call <c>ModelMetaDataProviders.Clear()</c>
    /// in your global.asax as the default provider is not compatible with the one in this library.
    /// </para>
    /// There are three available providers, flatfiles in the <c>Griffin.MvcConctrib.Localization.FlatFile</c> namespace
    /// and the external SqlServer package (which should work with any SQL based database) and finally the external package for RavenDb.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// Check for instance <see cref="ResourceStringProvider"/> to get a description about the actual localization process.
    /// </para>
    /// <para>You can either register the providers directly in your global.asax as:
    /// <example>
    /// <code>
    /// public class MvcApplication : System.Web.HttpApplication
    /// {
    ///     protected void Application_Start()
    ///     {
    ///         var stringProvider = new ResourceStringProvider(ModelMetadataStrings.ResourceManager);
    /// 
    ///         ModelMetadataProviders.Current = new LocalizedModelMetadataProvider(stringProvider);
    ///
    ///         // required when not using IoC.
    ///         ValidationMessageProviders.Clear();
    ///         ValidationMessageProviders.Add(new GriffinStringsProvider(st)); // the rest
    ///         ValidationMessageProviders.Add(new MvcDataSource()); //mvc attributes
    ///         ValidationMessageProviders.Add(new DataAnnotationDefaultStrings()); //data annotation attributes
    /// 
    ///         ModelValidatorProviders.Providers.Clear();
    ///         ModelValidatorProviders.Providers.Add(new LocalizedModelValidatorProvider());
    ///     }
    /// }
    /// </code>
    /// </example>
    /// Which is the easiest approach if your string provider doesn't depend on a connection to a database or similar. 
    /// 
    /// </para>
    /// <para>You can also register the providers in your container (the example uses Autofac):
    /// <example><code><![CDATA[
    /// // Loads strings from repositories.
    /// builder.RegisterType<RepositoryStringProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
    /// builder.RegisterType<ViewLocalizer>().AsImplementedInterfaces().InstancePerLifetimeScope();
    /// builder.RegisterType<SqlLocalizedTypesRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
    /// builder.RegisterType<SqlLocalizedViewsRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
    /// 
    /// // Connection factory
    /// builder.RegisterInstance(new AdoNetConnectionFactory("DemoDb")).AsSelf();
    /// builder.RegisterType<LocalizationDbContext>().AsImplementedInterfaces().InstancePerLifetimeScope(); 
    /// ]]></code></example>
    /// </para>
    /// <para>
    /// There is also an administration area <c>Griffin.MvcContrib.Admin</c> which can used to handle all translations.
    /// </para>
    /// </remarks>
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}