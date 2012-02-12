using Autofac;
using Autofac.Integration.Mvc;
using Griffin.MvcContrib.Areas.Griffin.Controllers;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.RavenDb.Localization;

namespace AdminTest.Modules
{
    public class LocalizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterControllers(typeof (LocalizeViewsController).Assembly);
            builder.RegisterType<LocalizedModelMetadataProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<LocalizedModelValidatorProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<LocalizedStringProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<TypeLocalizationRepository>().AsImplementedInterfaces();
            builder.RegisterType<ViewLocalizationRepository>().AsImplementedInterfaces();

            base.Load(builder);
        }
    }
}