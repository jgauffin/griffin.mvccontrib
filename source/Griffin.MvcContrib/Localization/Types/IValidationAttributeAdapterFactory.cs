using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Localization.Types
{
    /// <summary>
    /// Create validation rules for an attribute.
    /// </summary>
    public interface IValidationAttributeAdapterFactory
    {
        /// <summary>
        /// Generate client rules for a validation attribute
        /// </summary>
        /// <param name="attribute">Attribute to get rules for</param>
        /// <param name="errorMessage">Message to display</param>
        /// <returns>Validation rules</returns>
        IEnumerable<ModelClientValidationRule> Create(ValidationAttribute attribute, string errorMessage);
    }
}