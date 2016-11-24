using System.Globalization;
using Griffin.MvcContrib.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Griffin.MvcContrib.Tests.Localization
{
    [TestClass]
    public class DefaultCultureTests
    {
        [TestInitialize]
        public void ResetCulture()
        {
            DefaultUICulture.Reset();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(1033);
        }

        [TestMethod]
        public void DefaultLcidIsEnglish()
        {
            Assert.AreEqual(1033, DefaultUICulture.LCID);
        }

        [TestMethod]
        public void DefaultIsAlsoNeutral()
        {
            Assert.IsTrue(DefaultUICulture.Is(new CultureInfo("en")));
        }

        [TestMethod]
        public void DefaulIsUsEnglish()
        {
            Assert.IsTrue(DefaultUICulture.Is(new CultureInfo("en-us")));
        }

        [TestMethod]
        public void IsActive()
        {
            CultureInfo _culture = new CultureInfo(1033);
            Assert.IsTrue(DefaultUICulture.IsActive);
        }

        [TestMethod]
        public void ChangeCultureToGbAndVerify()
        {
            DefaultUICulture.Set(new CultureInfo("en-gb"));

            Assert.AreEqual(new CultureInfo("en-gb").LCID, DefaultUICulture.LCID);
            Assert.IsTrue(DefaultUICulture.Is(new CultureInfo("en")));
            Assert.IsTrue(DefaultUICulture.Is(new CultureInfo("en-gb")));
            Assert.IsFalse(DefaultUICulture.IsActive);
        }

        [TestMethod]
        public void ChangeCultureToSwedishAndVerify()
        {
            DefaultUICulture.Set(new CultureInfo(1053));

            Assert.AreEqual(1053, DefaultUICulture.LCID);
            Assert.IsFalse(DefaultUICulture.Is(new CultureInfo("en")));
            Assert.IsTrue(DefaultUICulture.Is(new CultureInfo(1053)));
            Assert.IsFalse(DefaultUICulture.IsActive);
            Assert.IsFalse(DefaultUICulture.IsEnglish);
        }
    }
}