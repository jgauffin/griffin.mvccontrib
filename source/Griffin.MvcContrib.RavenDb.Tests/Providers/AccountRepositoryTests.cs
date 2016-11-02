using System;
using Griffin.MvcContrib.Providers.Membership.SqlRepository;
using Griffin.MvcContrib.RavenDb.Providers;
using Raven.Client;
using Raven.Client.Converters;
using Raven.Client.Embedded;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Griffin.MvcContrib.RavenDb.Tests.Providers
{
    // integration tests
    [TestClass]
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

        [TestMethod]
        public void Register_GetByKey_Delete()
        {
            var email = Guid.NewGuid().ToString("N") + "@somewhere.com";
            var repos = new RavenDbAccountRepository(_session);
            var account = new MembershipAccount
                              {
                                  Email = email,
                                  UserName = email,
                                  Password = "clear text",
                                  ProviderUserKey = "ten"
                              };

            repos.Register(account);
            _session.SaveChanges();

            var user = repos.GetByProviderKey("ten");
            Assert.IsNotNull(user);

            repos.Delete(email, true);
            _session.SaveChanges();
        }

        [TestMethod]
        public void Register_GetByUsrName_Delete()
        {
            var email = Guid.NewGuid().ToString("N") + "@somewhere.com";
            var repos = new RavenDbAccountRepository(_session);
            var account = new MembershipAccount
            {
                Email = email,
                UserName = email,
                Password = "clear text",
                ProviderUserKey = "ten"
            };

            repos.Register(account);
            _session.SaveChanges();

            var user = repos.Get(email);
            Assert.IsNotNull(user);

            repos.Delete(email, true);
            _session.SaveChanges();
        }
        
        [TestMethod]
        public void ListUsers()
        {
            var repos = new RavenDbAccountRepository(_session);

            foreach (var account in _session.Query<AccountDocument>())
            {
                _session.Delete(account);
            }

            _session.SaveChanges();
            int records;
            repos.FindAll(1, 500, out records);
        }
    }
}