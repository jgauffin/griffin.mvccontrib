using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Localization.Views;
using Griffin.MvcContrib.SqlServer;
using Griffin.MvcContrib.SqlServer.Localization;
using Griffin.MvcContrib.VirtualPathProvider;

namespace Griffin.MvcContrib.Admin.TestProject
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private IContainer _container;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                new[] { typeof(Controllers.HomeController).Namespace }
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            // Disable role checking (user only have to be authenticated)
            GriffinAdminRoles.Translator = null;
            GriffinAdminRoles.HomePage = null;

            AddSupportForEmbeddedViews();
            SetupLocalicationProviders();

            var builder = new ContainerBuilder();
            builder.RegisterControllers(GetType().Assembly);
            RegisterLocalizationFeaturesInTheContainer(builder);
            _container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
        }

        private void SetupLocalicationProviders()
        {
            ModelValidatorProviders.Providers.Clear();
            ModelMetadataProviders.Current = new LocalizedModelMetadataProvider();
            ModelValidatorProviders.Providers.Add(new LocalizedModelValidatorProvider());

        }

        private static void RegisterLocalizationFeaturesInTheContainer(ContainerBuilder builder)
        {
            builder.RegisterControllers(typeof(MvcContrib.Areas.Griffin.GriffinAreaRegistration).Assembly);

            // Loads strings from repositories.
            builder.RegisterType<RepositoryStringProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ViewLocalizer>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SqlLocalizedTypesRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SqlLocalizedViewsRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();


            // Connection factory used by the SQL providers.
            builder.RegisterInstance(new AdoNetConnectionFactory("DemoDb")).AsSelf();
            builder.RegisterType<LocalizationDbContext>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }

        private static void AddSupportForEmbeddedViews()
        {
            // you can assign a custom WebViewPage or a custom layout in EmbeddedViewFixer.
            var fixer = new ExternalViewFixer()
                            {
                                LayoutPath = "~/Views/Shared/_Layout.cshtml"
                            };
            var provider = new EmbeddedViewFileProvider(VirtualPathUtility.ToAbsolute("~/"), fixer);
            provider.Add(new NamespaceMapping(typeof(MvcContrib.Areas.Griffin.GriffinAreaRegistration).Assembly, "Griffin.MvcContrib"));

            GriffinVirtualPathProvider.Current.Add(provider);
            HostingEnvironment.RegisterVirtualPathProvider(GriffinVirtualPathProvider.Current);
        }
    }
}