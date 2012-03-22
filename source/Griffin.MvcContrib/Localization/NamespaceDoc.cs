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
    /// global.asax. The easiest way to get prove strings for them is to use string tables with the help of <see cref="ResourceStringProvider"/> class.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Manage translations</term>
    /// <description>
    /// <para>
    /// You can also manage translations by using the <see cref="IViewLocalizationRepository"/> and the
    /// <see cref="ILocalizedTypesRepository"/> interfaces. Register one of the implementations in your inversion of control
    /// container. You might also want to register <see cref="LocalizedModelValidatorProvider"/> and <see cref="LocalizedModelMetadataProvider"/>
    /// in your container too, instead of assigning them directly (as they will need a database context). Dont forget to call <c>ModelMetaDataProviders.Clear()</c>
    /// in your global.asax as the default provider is not compatible with the one in this library.
    /// </para>
    /// There are three available providers, flatfiles in the "Localization.FlatFile" namespace
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
    ///          var stringProvider = new ResourceStringProvider(ModelMetadataStrings.ResourceManager);
    ///          ModelMetadataProviders.Current = new LocalizedModelMetadataProvider(stringProvider);
    ///
    ///         ModelValidatorProviders.Providers.Clear();
    ///         ModelValidatorProviders.Providers.Add(new LocalizedModelValidatorProvider(stringProvider));
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
    ///  builder.RegisterType<RepositoryStringProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
    ///  builder.RegisterType<ViewLocalizer>().AsImplementedInterfaces().InstancePerLifetimeScope();
    /// builder.RegisterType<SqlLocalizedTypesRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
    /// builder.RegisterType<SqlLocalizedViewsRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
    /// 
    /// // Localization providers.
    /// ModelValidatorProviders.Providers.Clear();
    /// builder.RegisterType<LocalizedModelMetadataProvider>().As<ModelMetadataProvider>().InstancePerLifetimeScope();
    /// builder.RegisterType<LocalizedModelValidatorProvider>().As<ModelValidatorProvider>().InstancePerLifetimeScope();
    /// 
    /// // Connection factory
    /// builder.RegisterInstance(new AdoNetConnectionFactory("DemoDb")).AsSelf();
    /// builder.RegisterType<LocalizationDbContext>().AsImplementedInterfaces().InstancePerLifetimeScope(); 
    /// ]]></code></example>
    /// Which works well for context sensitive implementations. Do note that you still need to invoke <c>ModelValidatorProviders.Providers.Clear();</c>
    /// as shown in the example.
    /// </para>    /// <para>
    /// There is also an administration area <c>Griffin.MvcContrib.Admin</c> which can used to handle all translations.
    /// </para>
    /// <para>
    /// !!IMPORTANT!! It seems like ASP.NET (MVC) doesnt honor the specified lifetime in the container (seems to keep an instance instead
    /// of resolving it again) for the ModelValidatorProvider and ModelMetadataProvider. This means that you can NOT register the providers
    /// in the container (but you can registering everything else since the providers uses DependencyResolver internally).
    /// </para>
    /// </remarks>
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}