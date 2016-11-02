using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Griffin.MvcContrib.Providers;

namespace Griffin.MvcContrib.Localization.Types
{
    /// <summary>
    /// creates adapters for client side validation
    /// </summary>
    public class ValidationAttributeAdapterFactory
    {
        private readonly Dictionary<Type, IValidationAttributeAdapterFactory> _factories =
            new Dictionary<Type, IValidationAttributeAdapterFactory>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttributeAdapterFactory"/> class.
        /// </summary>
        public ValidationAttributeAdapterFactory()
        {
            MapDefaultRules();
        }

        /// <summary>
        /// Maps the specified factory.
        /// </summary>
        /// <typeparam name="T">Validation attribute to return rules for</typeparam>
        /// <param name="factory">The factory.</param>
        /// <remarks>Replaces any existing factories.</remarks>
        public virtual void Map<T>(IValidationAttributeAdapterFactory factory) where T : ValidationAttribute
        {
            _factories[typeof (T)] = factory;
        }

        /// <summary>
        /// Create client validation rules for Data Annotation attributes.
        /// </summary>
        /// <param name="attribute">Attribute</param>
        /// <param name="errorMessage">Not formatted error message (should contain {0} etc}</param>
        /// <returns>A collection of rules (or an empty collection)</returns>
        public virtual IEnumerable<ModelClientValidationRule> Create(ValidationAttribute attribute, string errorMessage)
        {
            IValidationAttributeAdapterFactory factory;
            if (!_factories.TryGetValue(attribute.GetType(), out factory))
                return new ModelClientValidationRule[0];

            return factory.Create(attribute, errorMessage);
        }

        private void AddDelegateRule<T>(
            Func<ValidationAttribute, string, IEnumerable<ModelClientValidationRule>> factory)
            where T : ValidationAttribute
        {
            Map<T>(new DelegateValidationAttributeAdapterFactory(factory));
        }

        /// <summary>
        /// Map rules for the default attributes.
        /// </summary>
        protected virtual void MapDefaultRules()
        {
            AddDelegateRule<RangeAttribute>((attribute, errMsg) =>
                                                {
                                                    var attr = (RangeAttribute) attribute;
                                                    return new[]
                                                               {
                                                                   new ModelClientValidationRangeRule(errMsg,
                                                                                                      attr.Minimum,
                                                                                                      attr.Maximum)
                                                               };
                                                });
            AddDelegateRule<RegularExpressionAttribute>((attribute, errMsg) =>
                                                            {
                                                                var attr = (RegularExpressionAttribute) attribute;
                                                                return new[]
                                                                           {
                                                                               new ModelClientValidationRegexRule(
                                                                                   errMsg, attr.Pattern)
                                                                           };
                                                            });
            AddDelegateRule<RequiredAttribute>((attribute, errMsg) =>
                                                   {
                                                       var attr = (RequiredAttribute)attribute;
                                                       return new[]
                                                                  {
                                                                      new ModelClientValidationRequiredRule(errMsg)
                                                                  };
                                                   });
            AddDelegateRule<StringLengthAttribute>((attribute, errMsg) =>
                                                       {
                                                           var attr = (StringLengthAttribute) attribute;
                                                           return new[]
                                                                      {
                                                                          new ModelClientValidationStringLengthRule(
                                                                              errMsg, attr.MinimumLength,
                                                                              attr.MaximumLength)
                                                                      };
                                                       });
            AddDelegateRule<System.Web.Mvc.CompareAttribute>((attribute, errMsg) =>
                                                  {
                                                      var attr = (System.Web.Mvc.CompareAttribute) attribute;
                                                      return new[]
                                                                 {
                                                                     new ModelClientValidationEqualToRule(errMsg, System.Web.Mvc.CompareAttribute.FormatPropertyForClientValidation(attr.OtherProperty))
                                                                 };
                                                  });
            AddDelegateRule<System.ComponentModel.DataAnnotations.CompareAttribute>((attribute, errMsg) =>
            {
                var attr = (System.ComponentModel.DataAnnotations.CompareAttribute)attribute;
                return new[]
                           {
                                                                     new ModelClientValidationEqualToRule(errMsg, attr.OtherProperty)
                                                                 };
            });
        }
    }
}