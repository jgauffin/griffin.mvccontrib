using System;
using System.Globalization;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Griffin.MvcContrib;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Views;
using Griffin.MvcContrib.VirtualPathProvider;
using Localization.Resources;

namespace Localization
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : HttpApplication
	{
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
				new { controller = "User", action = "Index", id = UrlParameter.Optional } // Parameter defaults
				);
		}

		private IContainer _container;
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			HostingEnvironment.RegisterVirtualPathProvider(GriffinVirtualPathProvider.Current);
			//ViewEngines.Engines.Insert(0, new MyViewEngine());

			ControllerBuilder.Current.DefaultNamespaces.Add("Griffin.MvcContrib.Areas.Controller");
			//ControllerBuilder.Current.SetControllerFactory(new MyControllerFactory());

			var stringProvider = new ResourceStringProvider(LocalizedStrings.ResourceManager);
			ModelMetadataProviders.Current = new LocalizedModelMetadataProvider(stringProvider);
			ModelValidatorProviders.Providers.Clear();
			ModelValidatorProviders.Providers.Add(new LocalizedModelValidatorProvider(stringProvider));

			ViewLocalizer.DefaultCulture = new CultureInfo(1053);

			var builder = new ContainerBuilder();
			builder.RegisterControllers(Assembly.GetExecutingAssembly());

			builder.RegisterType<ViewLocalizationFileRepository>().AsImplementedInterfaces();
			builder.RegisterType<CustomControllerActivator>().AsImplementedInterfaces().SingleInstance();
			_container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));


			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}
	}

	public class CustomControllerActivator : IControllerActivator
	{
		IController IControllerActivator.Create(System.Web.Routing.RequestContext requestContext, Type controllerType)
		{
			return DependencyResolver.Current.GetService(controllerType) as IController;
		}
	}
	/*
	public class MyControllerFactory : DefaultControllerFactory
	{
		public override IController CreateController(RequestContext requestContext, string controllerName)
		{
			var controllerType =
				Type.GetType("Griffin.MvcContrib.Areas.Controller." + controllerName + "Controller, Griffin.MvcContrib", false, true);
			if (controllerType != null)
				return (IController) DependencyResolver.Current.GetService(controllerType);

			return base.CreateController(requestContext, controllerName);
		}
	}
	*/
}