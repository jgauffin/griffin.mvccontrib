using System.Globalization;
using Griffin.MvcContrib.Localization;
using Xunit;

namespace Griffin.MvcContrib.Tests.Localization
{
    public class DefaultCultureTests
    {
        public DefaultCultureTests()
        {
            DefaultUICulture.Reset();
        }

        [Fact]
        public void DefaultLcidIsEnglish()
        {
            Assert.Equal(1033, DefaultUICulture.LCID);
        }

        [Fact]
        public void DefaultIsAlsoNeutral()
        {
            Assert.True(DefaultUICulture.Is(new CultureInfo("en")));
        }

        [Fact]
        public void DefaulIsUsEnglish()
        {
            Assert.True(DefaultUICulture.Is(new CultureInfo("en-us")));
        }

        [Fact]
        public void IsActive()
        {
            Assert.True(DefaultUICulture.IsActive);
        }

        [Fact]
        public void ChangeCultureToGbAndVerify()
        {
            DefaultUICulture.Set(new CultureInfo("en-gb"));

            Assert.Equal(new CultureInfo("en-gb").LCID, DefaultUICulture.LCID);
            Assert.True(DefaultUICulture.Is(new CultureInfo("en")));
            Assert.True(DefaultUICulture.Is(new CultureInfo("en-gb")));
            Assert.False(DefaultUICulture.IsActive);
        }

        [Fact]
        public void ChangeCultureToSwedishAndVerify()
        {
            DefaultUICulture.Set(new CultureInfo(1053));

            Assert.Equal(1053, DefaultUICulture.LCID);
            Assert.False(DefaultUICulture.Is(new CultureInfo("en")));
            Assert.True(DefaultUICulture.Is(new CultureInfo(1053)));
            Assert.False(DefaultUICulture.IsActive);
            Assert.False(DefaultUICulture.IsEnglish);
        }
    }
}