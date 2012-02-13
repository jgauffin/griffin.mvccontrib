using System;
using Griffin.MvcContrib.Providers.Membership.SqlRepository;
using Griffin.MvcContrib.RavenDb.Providers;
using Raven.Client;
using Raven.Client.Embedded;
using Xunit;

namespace Griffin.MvcContrib.RavenDb.Tests.Providers
{
    // integration tests

    public class AccountRepositoryTests : IDisposable
    {
        private readonly EmbeddableDocumentStore _documentStore;
        private readonly IDocumentSession _session;

        public AccountRepositoryTests()
        {
            _documentStore = new EmbeddableDocumentStore {Conventions = {IdentityPartsSeparator = "-"}};
            _documentStore.Initialize();
            _session = _documentStore.OpenSession();
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            _session.Dispose();
            _documentStore.Dispose();
        }

        #endregion

        [Fact]
        public void Register()
        {
            var repos = new RavenDbAccountRepository(_session);
            repos.Register(new MembershipAccount
                               {
                                   Email = "some@name.com",
                                   Password = "clear text"
                               });
        }
    }
}