using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Localization.Types
{
    /// <summary>
    /// Uses a delegate to create the client validation rules.
    /// </summary>
    public class DelegateValidationAttributeAdapterFactory : IValidationAttributeAdapterFactory
    {
        private readonly Func<ValidationAttribute, string, IEnumerable<ModelClientValidationRule>> _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateValidationAttributeAdapterFactory"/> class.
        /// </summary>
        /// <param name="factory">Takes attribute + error Message and returns client rules.</param>
        public DelegateValidationAttributeAdapterFactory(Func<ValidationAttribute, string, IEnumerable<ModelClientValidationRule>> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Generate client rules for a validation attribute
        /// </summary>
        /// <param name="attribute">Attribute to get rules for</param>
        /// <param name="errorMessage">Message to display</param>
        /// <returns>Validation rules</returns>
        public IEnumerable<ModelClientValidationRule> Create(ValidationAttribute attribute, string errorMessage)
        {
            return _factory(attribute, errorMessage);
        }
    }
}