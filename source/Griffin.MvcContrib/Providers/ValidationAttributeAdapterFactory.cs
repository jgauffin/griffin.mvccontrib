using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Providers
{
    /// <summary>
    /// creates adapters for client side validation
    /// </summary>
    public class ValidationAttributeAdapterFactory
    {
        /// <summary>
        /// Create client validation rules for Data Annotation attributes.
        /// </summary>
        /// <param name="attribute">Attribute</param>
        /// <param name="errorMessage">Not formatted error message (should contain {0} etc}</param>
        /// <returns>A collection of rules (or an empty collection)</returns>
        public virtual IEnumerable<ModelClientValidationRule> Create(ValidationAttribute attribute, string errorMessage)
        {
            if (attribute is RangeAttribute)
            {
                var attr = (RangeAttribute) attribute;
                return new[]
                           {
                               new ModelClientValidationRangeRule(errorMessage, attr.Minimum, attr.Maximum)
                           };
            }
            if (attribute is RegularExpressionAttribute)
            {
                var attr = (RegularExpressionAttribute) attribute;
                return new[] {new ModelClientValidationRegexRule(errorMessage, attr.Pattern)};
            }
            if (attribute is RequiredAttribute)
            {
                var attr = (RequiredAttribute) attribute;
                return new[] {new ModelClientValidationRequiredRule(errorMessage)};
            }
            if (attribute is StringLengthAttribute)
            {
                var attr = (StringLengthAttribute) attribute;
                return new[]
                           {
                               new ModelClientValidationStringLengthRule(errorMessage, attr.MinimumLength,
                                                                         attr.MaximumLength)
                           };
            }

            return new ModelClientValidationRule[0];
        }
    }
}