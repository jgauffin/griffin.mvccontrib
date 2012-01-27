using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Localization
{
	/// <summary>
	/// creates adapters for client side validation
	/// </summary>
	public class ValidationAttributeAdapterFactory
	{
		public IEnumerable<ModelClientValidationRule> Create(ValidationAttribute attribute, string errorMessage)
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
				return new[] { new ModelClientValidationRequiredRule(errorMessage) };
			}
			if (attribute is StringLengthAttribute)
			{
				var attr = (StringLengthAttribute) attribute;
				return new[] { new ModelClientValidationStringLengthRule(errorMessage, attr.MinimumLength, attr.MaximumLength) };
			}

			return new ModelClientValidationRule[0];
		}
	}
}
