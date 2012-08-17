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
                k.GetModelString(It.Is<Type>(t => t == typeof (TestModel)), It.Is<string>(t => t == "Required"))).
                Returns("Användarnamn").Verifiable();

            var subject = new TestModel
                              {
                                  RequiredStringLength10 = "Arne",
                                  Required = "hej"
                              };

            var actual = provider.GetMetadataForProperty(() => subject, typeof (TestModel), "Required");

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
                k.GetModelString(It.Is<Type>(t => t == typeof (TestModel)), It.Is<string>(t => t == "RequiredStringLength10"))).
                Returns("Efternamn").Verifiable();

            var subject = new TestModel
                              {
                                  RequiredStringLength10 = "Arne",
                                  Required = "hej"
                              };

            var actual = provider.GetMetadataForProperty(() => subject, typeof (TestModel), "RequiredStringLength10");

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
                k.GetModelString(It.Is<Type>(t => t == typeof (TestModel)), It.Is<string>(t => t == "Required"))).
                Returns((string) null).Verifiable();

            var subject = new TestModel
                              {
                                  RequiredStringLength10 = "Arne",
                                  Required = "hej"
                              };

            var actual = provider.GetMetadataForProperty(() => subject, typeof (TestModel), "Required");

            Assert.Equal(null, actual.DisplayName);
        }
    }
}