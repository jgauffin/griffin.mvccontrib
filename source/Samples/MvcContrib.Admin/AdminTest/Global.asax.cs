using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using AdminTest.Modules;
using Autofac;
using Autofac.Integration.Mvc;
using Griffin.MvcContrib;
using Griffin.MvcContrib.Areas.Griffin.Controllers;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Providers.Membership;
using Griffin.MvcContrib.RavenDb.Localization;
using Griffin.MvcContrib.VirtualPathProvider;
using Raven.Client.Embedded;

namespace AdminTest
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
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
			RegisterContainer();
		    HostingEnvironment.RegisterVirtualPathProvider(GriffinVirtualPathProvider.Current);

		    var embeddedProvider = new EmbeddedViewFileProvider();
		    embeddedProvider.Add(new NamespaceMapping(typeof (GriffinHomeController).Assembly, "Griffin.MvcContrib"));
            GriffinVirtualPathProvider.Current.Add(embeddedProvider);

			var stringProvider = _container.Resolve<ILocalizedStringProvider>();
			ModelMetadataProviders.Current = new LocalizedModelMetadataProvider(stringProvider);

			ModelValidatorProviders.Providers.Clear();
			ModelValidatorProviders.Providers.Add(new LocalizedModelValidatorProvider(stringProvider));

		}

		private void RegisterContainer()
		{
			var builder = new ContainerBuilder();
			builder.RegisterControllers(Assembly.GetExecutingAssembly());
			builder.RegisterModules(Assembly.GetExecutingAssembly());
		    builder.RegisterType<EmbeddedViewFixer>().AsImplementedInterfaces().SingleInstance();
			_container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
		}
	}
}