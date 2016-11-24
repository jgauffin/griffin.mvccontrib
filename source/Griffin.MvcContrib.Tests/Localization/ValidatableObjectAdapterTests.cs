using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Griffin.MvcContrib.Localization.ValidationMessages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Griffin.MvcContrib.Tests.Localization
{
    [TestClass]
    public class ValidatableObjectAdapterTests
    {
        [TestMethod]
        public void ValidatbleObjectAdapter()
        {
            var model = new TestModel();
            var k = new DataAnnotationsModelMetadataProvider();
            var metadata = k.GetMetadataForType(() => model, model.GetType());
            var controllerContext = new ControllerContext();
            var adapter = new MvcContrib.Localization.ValidatableObjectAdapter(metadata, controllerContext);

            var result = adapter.Validate(model).ToList();
            Assert.AreNotEqual(0, result.Count);
        }
    }
}
