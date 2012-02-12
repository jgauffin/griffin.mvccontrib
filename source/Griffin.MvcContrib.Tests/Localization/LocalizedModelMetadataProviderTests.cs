using System;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using Moq;
using Xunit;

namespace Griffin.MvcContrib.Tests.Localization
{
    public class LocalizedModelMetadataProviderTests
    {
        [Fact]
        public void TestAProperty()
        {
            var stringProvider = new Mock<ILocalizedStringProvider>();
            var provider = new LocalizedModelMetadataProvider(stringProvider.Object);
            stringProvider.Setup(
                k =>
                k.GetModelString(It.Is<Type>(t => t == typeof (MetadataTarget)), It.Is<string>(t => t == "UserName"))).
                Returns("Användarnamn").Verifiable();

            var subject = new MetadataTarget
                              {
                                  LastName = "Arne",
                                  UserName = "hej"
                              };

            var actual = provider.GetMetadataForProperty(() => subject, typeof (MetadataTarget), "UserName");

            Assert.Equal("Användarnamn", actual.DisplayName);
            stringProvider.VerifyAll();
        }

        [Fact]
        public void TestAnotherProperty()
        {
            var stringProvider = new Mock<ILocalizedStringProvider>();
            var provider = new LocalizedModelMetadataProvider(stringProvider.Object);
            stringProvider.Setup(
                k =>
                k.GetModelString(It.Is<Type>(t => t == typeof (MetadataTarget)), It.Is<string>(t => t == "LastName"))).
                Returns("Efternamn").Verifiable();

            var subject = new MetadataTarget
                              {
                                  LastName = "Arne",
                                  UserName = "hej"
                              };

            var actual = provider.GetMetadataForProperty(() => subject, typeof (MetadataTarget), "LastName");

            Assert.Equal("Efternamn", actual.DisplayName);
            stringProvider.VerifyAll();
        }

        [Fact]
        public void TestNotTranslatedProperty()
        {
            DefaultUICulture.Reset();
            var stringProvider = new Mock<ILocalizedStringProvider>();
            var provider = new LocalizedModelMetadataProvider(stringProvider.Object);
            stringProvider.Setup(
                k =>
                k.GetModelString(It.Is<Type>(t => t == typeof (MetadataTarget)), It.Is<string>(t => t == "UserName"))).
                Returns((string) null).Verifiable();

            var subject = new MetadataTarget
                              {
                                  LastName = "Arne",
                                  UserName = "hej"
                              };

            var actual = provider.GetMetadataForProperty(() => subject, typeof (MetadataTarget), "UserName");

            Assert.Equal(null, actual.DisplayName);
        }
    }
}