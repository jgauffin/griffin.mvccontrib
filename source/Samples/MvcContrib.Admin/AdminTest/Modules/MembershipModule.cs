using Autofac;
using Griffin.MvcContrib.Providers.Membership;
using Griffin.MvcContrib.Providers.Membership.PasswordStrategies;
using Griffin.MvcContrib.RavenDb.Providers;

namespace AdminTest.Modules
{
	public class MembershipModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<MembershipProvider>().AsImplementedInterfaces();
			builder.RegisterType<HashPasswordStrategy>().
				AsImplementedInterfaces().SingleInstance();
			builder.RegisterInstance(new PasswordPolicy()).AsImplementedInterfaces();
			builder.RegisterType<RavenDbAccountRepository>().AsImplementedInterfaces();

			base.Load(builder);
		}
	}
}