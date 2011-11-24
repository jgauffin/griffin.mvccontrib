Griffin.MvcContrib.Admin v0.1b

You need to configure the following in your Global.Asax to get the localization area running:

	ModelMetadataProviders.Current = new LocalizedModelMetadataProvider();
	ModelValidatorProviders.Providers.Clear();
	ModelValidatorProviders.Providers.Add(new LocalizedModelValidatorProvider());


Both need a ILocalizedStringProvider to work. Either pass one in the contructor or register it 
in your inversion of control container.

More info about the providers: http://blog.gauffin.org/2011/09/easy-model-and-validation-localization-in-asp-net-mvc3/

--------------

To use the account administratoin area you'll need to configure the following:

	builder.RegisterType<RavenDb.Providers.RavenDbAccountRepository>().AsImplementedInterfaces();
	builder.RegisterType<RavenDb.Providers.RavenDbRoleRepository>().AsImplementedInterfaces();
	builder.RegisterType<Providers.Membership.PasswordStrategies.HashPasswordStrategy>().AsImplementedInterfaces();
	builder.RegisterType<PasswordPolicy>().AsImplementedInterfaces();

(or implement IAccountRepository by yourself)

More info about the provider: http://blog.gauffin.org/2011/09/a-more-structured-membershipprovider/


-------------

