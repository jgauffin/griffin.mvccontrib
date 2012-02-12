using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using Moq;
using Xunit;

namespace Griffin.MvcContrib.Tests.Localization
{
    public class LocalizedModelValidatorProviderTests
    {
        private readonly LocalizedModelValidatorProvider _provider;
        private readonly Mock<ILocalizedStringProvider> _stringProvider;

        public LocalizedModelValidatorProviderTests()
        {
            _stringProvider = new Mock<ILocalizedStringProvider>();
            _provider = new LocalizedModelValidatorProvider(_stringProvider.Object);
        }

        [Fact]
        public void DefaultRequiredClientValidationString()
        {
            var model = new MetadataTarget {LastName = "Arne", UserName = "Kalle"};
            var metadataProvider = new DataAnnotationsModelMetadataProvider();
            var metadata = metadataProvider.GetMetadataForProperty(() => model, typeof (MetadataTarget), "UserName");

            var validator =
                _provider.GetValidators(metadata, new ControllerContext(new RequestContext(), new SomeController())).
                    Single();

            var clientRules = validator.GetClientValidationRules();

            Assert.True(validator.IsRequired);
            Assert.Equal("The UserName field is required.", clientRules.Single().ErrorMessage);
        }

        [Fact]
        public void RequiredClientValidationString()
        {
            var model = new MetadataTarget {LastName = "Arne", UserName = "Kalle"};
            var metadataProvider = new DataAnnotationsModelMetadataProvider();
            var metadata = metadataProvider.GetMetadataForProperty(() => model, typeof (MetadataTarget), "UserName");
            _stringProvider.Setup(k => k.GetValidationString(typeof (RequiredAttribute))).Returns(
                "Fältet '{0}' är humm!").Verifiable();

            var validator =
                _provider.GetValidators(metadata, new ControllerContext(new RequestContext(), new SomeController())).
                    Single();

            var clientRules = validator.GetClientValidationRules();

            Assert.True(validator.IsRequired);
            Assert.Equal("Fältet 'UserName' är humm!", clientRules.Single().ErrorMessage);
            _stringProvider.VerifyAll();
        }
    }

    internal class SomeController : Controller
    {
    }
}