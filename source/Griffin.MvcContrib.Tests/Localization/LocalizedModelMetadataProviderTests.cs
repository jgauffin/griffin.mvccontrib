using System;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Threading;

namespace Griffin.MvcContrib.Tests.Localization
{
    [TestClass]
    public class LocalizedModelMetadataProviderTests
    {
        [TestInitialize]
        public void CultureReset()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(1033);
        }

        [TestMethod]
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

            Assert.AreEqual("Användarnamn", actual.DisplayName);
            stringProvider.VerifyAll();
        }

        [TestMethod]
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

            Assert.AreEqual("Efternamn", actual.DisplayName);
            stringProvider.VerifyAll();
        }

        [TestMethod]
        public void TestNotTranslatedProperty()
        {
            DefaultUICulture.Reset();

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(1033);
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

            Assert.AreEqual("[?: Required]", actual.DisplayName);
        }
    }
}