using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Localization
{
    /// <summary>
    /// Used to localize DatAnnotation attribute error messages
    /// </summary>
    /// <remarks>
    /// Check for instance <see cref="ResourceStringProvider"/> to get a description about the actual localization process.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class MvcApplication : System.Web.HttpApplication
    /// {
    ///     protected void Application_Start()
    ///     {
    ///         var stringProvider = new ResourceStringProvider(ModelMetadataStrings.ResourceManager);
    ///         ModelValidatorProviders.Providers.Clear();
    ///         ModelValidatorProviders.Providers.Add(new LocalizedModelValidatorProvider(stringProvider));
    ///     }
    /// }
    /// </code>
    /// </example>
    public class LocalizedModelValidatorProvider : DataAnnotationsModelValidatorProvider 
    {
        private readonly ILocalizedStringProvider _stringProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedModelValidatorProvider"/> class.
        /// </summary>
        /// <param name="stringProvider">The string provider.</param>
        public LocalizedModelValidatorProvider(ILocalizedStringProvider stringProvider)
        {
            _stringProvider = stringProvider;
        }

        /// <summary>
        /// Gets a list of validators.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="context">The context.</param>
        /// <param name="attributes">The list of validation attributes.</param>
        /// <returns>
        /// A list of validators.
        /// </returns>
        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes)
        {
            foreach (var attr in attributes.OfType<ValidationAttribute>())
                attr.ErrorMessage = _stringProvider.GetValidationString(attr.GetType());
            return base.GetValidators(metadata, context, attributes);
        }
    }
}