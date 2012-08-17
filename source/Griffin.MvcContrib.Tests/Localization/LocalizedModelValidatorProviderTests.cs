using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Localization.ValidationMessages;
using Moq;
using Xunit;

namespace Griffin.MvcContrib.Tests.Localization
{
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

        [Fact]
        public void CustomErrorMessage()
        {
            var model = new TestModel{Compare1 = "1", Compare2 = "b", Required = "yes", RequiredStringLength10 = "qaa", ResourceFile = "nope"};
            var validator = GetValidator(model, "CustomMessage");

            var actual = validator.Validate(model).First().Message;
            Assert.Equal("Custom message", actual);
        }

        [Fact]
        public void RegEx()
        {
            ValidationMessageProviders.Clear();
            ValidationMessageProviders.Add(new DataAnnotationDefaultStrings());
            var model = new TestModel { RegEx = "word"};
            var validator = GetValidator(model, "RegEx");

            var actual = validator.Validate(model).First().Message;

            // regex can't be identified in the DataAnnotations resource file :(
            Assert.Equal("[en-US: RegularExpression]", actual);
        }

        [Fact]
        public void ResourceFileMessage()
        {
            var model = new TestModel();
            var validator = GetValidator(model, "ResourceFile");

            var actual = validator.Validate(model).First().Message;
            Assert.Equal("From resource file", actual);
        }

        [Fact]
        public void DefaultMessage()
        {
            ValidationMessageProviders.Clear();
            ValidationMessageProviders.Add(new DataAnnotationDefaultStrings());
            var model = new TestModel();
            var validator = GetValidator(model, "Required");

            var actual = validator.Validate(model).First().Message;
            Assert.Equal("The Required field is required.", actual);
        }

        [Fact]
        public void CompareAttribute()
        {
            ValidationMessageProviders.Clear();
            ValidationMessageProviders.Add(new MvcDataSource());
            var model = new TestModel();
            model.Compare2 = "Differ";
            model.Compare1 = "Aaa";
            var validator = GetValidator(model, "Compare1");

            var actual = validator.Validate(model).First().Message;
            Assert.Equal("The Compare1 and Compare2 fields to not match.", actual);
        }



        [Fact]
        public void ClientValidatable()
        {
            ValidationMessageProviders.Reset();
            var model = new TestModel();
            var k = new DataAnnotationsModelMetadataProvider();
            var metadata = k.GetMetadataForType(() => model, model.GetType());
            var validators = _provider.GetValidators(metadata, new ControllerContext());

            var result = validators.ToList().First().Validate(model).ToList();

            Assert.NotEqual(0, result.Count);

        }

        private ModelValidator GetValidator(TestModel model, string propertyName)
        {
            var metadataProvider = new DataAnnotationsModelMetadataProvider();
            var metadata =
                metadataProvider.GetMetadataForProperties(model, model.GetType()).Single(x => x.PropertyName == propertyName);

            var validator =
                _provider.GetValidators(metadata, new ControllerContext(new RequestContext(), new SomeController())).
                    Single();
            return validator;
        }

        [Fact]
        public void DefaultRequiredClientValidationString()
        {
            var model = new TestModel { RequiredStringLength10 = "Arne", Required = "Kalle" };
            var metadataProvider = new DataAnnotationsModelMetadataProvider();
            var metadata = metadataProvider.GetMetadataForProperty(() => model, typeof(TestModel), "Required");
            _stringProvider.Setup(k => k.GetMessage(It.Is<IGetMessageContext>(x => x.Attribute is RequiredAttribute))).Returns(
                "The {0} field is required.").Verifiable();

            var validator =
                _provider.GetValidators(metadata, new ControllerContext(new RequestContext(), new SomeController())).
                    First();

            var clientRules = validator.GetClientValidationRules();

            Assert.True(validator.IsRequired);
            Assert.Equal("The Required field is required.", clientRules.Single().ErrorMessage);
        }

        [Fact]
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

            Assert.True(validator.IsRequired);
            Assert.Equal("Fältet 'Required' är humm!", clientRules.Single().ErrorMessage);
            _stringProvider.VerifyAll();
        }
    }

    internal class SomeController : Controller
    {
    }
}