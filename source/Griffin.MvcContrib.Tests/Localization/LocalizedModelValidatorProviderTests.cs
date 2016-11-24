using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Localization.ValidationMessages;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Globalization;

namespace Griffin.MvcContrib.Tests.Localization
{
    [TestClass]
    public class LocalizedModelValidatorProviderTests
    {
        private readonly LocalizedModelValidatorProvider _provider;
        private readonly Mock<IValidationMessageDataSource> _stringProvider;
        
        public LocalizedModelValidatorProviderTests()
        {
            _stringProvider = new Mock<IValidationMessageDataSource>();
            ValidationMessageProviders.Clear();
            ValidationMessageProviders.Add(_stringProvider.Object);
            _provider = new LocalizedModelValidatorProvider();
        }

        [TestInitialize]
        public void SetCurrentLocale()
        {

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(1033);
        }

        [TestMethod]
        public void CustomErrorMessage()
        {
            var model = new TestModel{Compare1 = "1", Compare2 = "b", Required = "yes", RequiredStringLength10 = "qaa", ResourceFile = "nope"};
            var validator = GetValidator(model, "CustomMessage");

            var actual = validator.Validate(model).First().Message;
            Assert.AreEqual("Custom message", actual);
        }

        [TestMethod]
        public void RegEx()
        {
            ValidationMessageProviders.Clear();
            ValidationMessageProviders.Add(new DataAnnotationDefaultStrings());
            var model = new TestModel { RegEx = "word"};
            var validator = GetValidator(model, "RegEx");

            var actual = validator.Validate(model).First().Message;

            // regex can't be identified in the DataAnnotations resource file :(
            Assert.AreEqual("[en-US: RegularExpression]", actual);
        }

        [TestMethod]
        public void ResourceFileMessage()
        {
            var model = new TestModel();
            var validator = GetValidator(model, "ResourceFile");

            var actual = validator.Validate(model).First().Message;
            Assert.AreEqual("From resource file", actual);
        }

        [TestMethod]
        public void DefaultMessage()
        {
            ValidationMessageProviders.Clear();
            ValidationMessageProviders.Add(new DataAnnotationDefaultStrings());
            var model = new TestModel();
            var validator = GetValidator(model, "Required");

            var actual = validator.Validate(model).First().Message;
            Assert.AreEqual("The Required field is required.", actual);
        }

        [TestMethod]
        public void CompareAttribute()
        {
            ValidationMessageProviders.Clear();
            ValidationMessageProviders.Add(new MvcDataSource());
            var model = new TestModel();
            model.Compare2 = "Differ";
            model.Compare1 = "Aaa";
            var validator = GetValidator(model, "Compare1");

            var actual = validator.Validate(model).First().Message;
            Assert.AreEqual("The Compare1 and Compare2 fields to not match.", actual);
        }



        [TestMethod]
        public void ClientValidatable()
        {
            ValidationMessageProviders.Reset();
            var model = new TestModel();
            var k = new DataAnnotationsModelMetadataProvider();
            var metadata = k.GetMetadataForType(() => model, model.GetType());
            var validators = _provider.GetValidators(metadata, new ControllerContext());

            var result = validators.ToList().First().Validate(model).ToList();

            Assert.AreNotEqual(0, result.Count);

        }

        private System.Web.Mvc.ModelValidator GetValidator(TestModel model, string propertyName)
        {
            var metadataProvider = new DataAnnotationsModelMetadataProvider();
            var metadata =
                metadataProvider.GetMetadataForProperties(model, model.GetType()).Single(x => x.PropertyName == propertyName);

            var validator =
                _provider.GetValidators(metadata, new ControllerContext(new RequestContext(), new SomeController())).
                    Single();
            return validator;
        }

        [TestMethod]
        public void DefaultRequiredClientValidationString()
        {
            var model = new TestModel { RequiredStringLength10 = "Arne", Required = "Kalle" };
            var metadataProvider = new System.Web.Mvc.DataAnnotationsModelMetadataProvider();
            var metadata = metadataProvider.GetMetadataForProperty(() => model, typeof(TestModel), "Required");
            _stringProvider.Setup(k => k.GetMessage(It.Is<IGetMessageContext>(x => x.Attribute is RequiredAttribute))).Returns(
                "The {0} field is required.").Verifiable();

            var validator =
                _provider.GetValidators(metadata, new ControllerContext(new RequestContext(), new SomeController())).
                    First();

            var clientRules = validator.GetClientValidationRules();

            Assert.IsTrue(validator.IsRequired);
            Assert.AreEqual("The Required field is required.", clientRules.Single().ErrorMessage);
        }

        [TestMethod]
        public void RequiredClientValidationString()
        {
            var model = new TestModel { RequiredStringLength10 = "Arne", Required = "Kalle" };
            var metadataProvider = new DataAnnotationsModelMetadataProvider();
            var metadata = metadataProvider.GetMetadataForProperty(() => model, typeof(TestModel), "Required");
            _stringProvider.Setup(k => k.GetMessage(It.Is<IGetMessageContext>(x => x.Attribute is RequiredAttribute))).Returns(
                "Fältet '{0}' är humm!").Verifiable();

            var validator =
                _provider.GetValidators(metadata, new ControllerContext(new RequestContext(), new SomeController())).
                    First();

            var clientRules = validator.GetClientValidationRules();

            Assert.IsTrue(validator.IsRequired);
            Assert.AreEqual("Fältet 'Required' är humm!", clientRules.Single().ErrorMessage);
            _stringProvider.VerifyAll();
        }
    }

    internal class SomeController : Controller
    {
    }
}