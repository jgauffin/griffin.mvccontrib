using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Providers.Membership;
using Griffin.MvcContrib.RavenDb.Localization;
using Raven.Client;
using Raven.Client.Embedded;

namespace Griffin.MvcContrib
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
				new[] { typeof(MvcApplication).Namespace + ".Controllers" }
			);

		}

		protected void Application_Start()
		{
			LogProvider.UseDebugWindow();
			AreaRegistration.RegisterAllAreas();
			CreateContainer();

			ModelMetadataProviders.Current = new LocalizedModelMetadataProvider();
			ModelValidatorProviders.Providers.Clear();
			ModelValidatorProviders.Providers.Add(new LocalizedModelValidatorProvider());

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}

		private void CreateContainer()
		{
			var builder = new ContainerBuilder();
			builder.RegisterControllers(Assembly.GetExecutingAssembly());

			//localization
			builder.RegisterType<ViewLocalizationRepository>().AsImplementedInterfaces();
			builder.RegisterType<TypeLocalizationRepository>().AsImplementedInterfaces();

			//membership
			builder.RegisterType<RavenDb.Providers.RavenDbAccountRepository>().AsImplementedInterfaces();
			builder.RegisterType<RavenDb.Providers.RavenDbRoleRepository>().AsImplementedInterfaces();
			builder.RegisterType<Providers.Membership.PasswordStrategies.HashPasswordStrategy>().AsImplementedInterfaces();
			builder.RegisterType<PasswordPolicy>().AsImplementedInterfaces();

			// database
			var docStore = new EmbeddableDocumentStore();
			docStore.Initialize();
			builder.Register(p => docStore.OpenSession()).As<IDocumentSession>().InstancePerLifetimeScope();

			// 

			_container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
		}
	}
}