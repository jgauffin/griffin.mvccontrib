using Autofac;
using Autofac.Integration.Mvc;
using Griffin.MvcContrib.Areas.Griffin.Controllers;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Localization.Views;

namespace SqlServerLocalization.Modules
{
	public class LocalizationModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterControllers(typeof(LocalizeViewsController).Assembly);
			builder.RegisterType<LocalizedModelMetadataProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
			builder.RegisterType<LocalizedModelValidatorProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
			builder.RegisterType<LocalizedStringProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
		    builder.RegisterType<RepositoryViewLocalizer>().AsImplementedInterfaces().InstancePerHttpRequest();
			base.Load(builder);
		}
	}
}