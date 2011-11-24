using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Raven.Client;
using Raven.Client.Embedded;

namespace AdminTest.Modules
{
	public class RavenModule : Module
	{
		private EmbeddableDocumentStore _documentStore;

		protected override void Load(ContainerBuilder builder)
		{
			_documentStore = new EmbeddableDocumentStore {Conventions = {IdentityPartsSeparator = "-"}};
			_documentStore.Initialize();

			builder.Register(c => _documentStore.OpenSession()).As<IDocumentSession>().InstancePerLifetimeScope();

			base.Load(builder);
		}
	}
}