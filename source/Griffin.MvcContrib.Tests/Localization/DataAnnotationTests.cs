using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Griffin.MvcContrib.Localization.ValidationMessages;
using Xunit;

namespace Griffin.MvcContrib.Tests.Localization
{
    public class DataAnnotationTests
    {
        public string GetMessage<T>(string propertyName, int lcid) where T : ValidationAttribute, new()
        {
            var da = new DataAnnotationDefaultStrings();

            var context = new GetMessageContext(new T(), typeof(TestModel), propertyName,
                                                new CultureInfo(lcid));

            return da.GetMessage(context);
        }

        [Fact]
        public void GetRequiredEnglish()
        {
            var message = GetMessage<RequiredAttribute>("Required", new CultureInfo("en-us").LCID);
            Assert.Equal("The {0} field is required.", message);
        }

        [Fact]
        public void GetRequiredUnknownLang()
        {
            var message = GetMessage<RequiredAttribute>("Required", 1098);
            Assert.Equal(null, message);
        }

    }
}
