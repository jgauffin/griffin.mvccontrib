/*
 * Copyright (c) 2011, Jonas Gauffin. All rights reserved.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301 USA
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Localization.ValidationMessages;
using Griffin.MvcContrib.Logging;

namespace Griffin.MvcContrib.Localization
{
    /// <summary>
    /// Used to localize DataAnnotation attribute error messages and view models
    /// </summary>
    /// <remarks>
    /// <para>Hacks the attributes by assigning custom (localized) messages to them to get localized error messages.</para>
    /// <para>
    /// Check for namespace documentation for an example on how to use the provider.
    /// </para>
    /// <para>Are you missing validation rules for an attribute? Do not try to use the original validation rules. The standard attributes
    /// uses some nasty delegates to handle the error message. Screwing with them should be handled with care. 
    /// </para>
    /// <para>Create a new <see cref="IValidationMessageDataSource"/> and register it in <see cref="ValidationMessageProviders"/> to customized the translated strings.</para>
    /// <para>You have to let the results returned from <c>Validate()</c> implement <see cref="IClientValidationRule"/> if you want to enable client validation when using <see cref="IValidatableObject"/>.</para>
    /// </remarks>
    public class LocalizedModelValidatorProvider : DataAnnotationsModelValidatorProvider, IDisposable
    {
        private const string WorkaroundMarker = "#g#";
        private readonly ValidationAttributeAdapterFactory _adapterFactory = new ValidationAttributeAdapterFactory();
        private readonly ILogger _logger = LogProvider.Current.GetLogger<LocalizedModelValidatorProvider>();

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

        /// <summary>
        /// Gets a list of validators.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="context">The context.</param>
        /// <param name="attributes">The list of validation attributes.</param>
        /// <returns>
        /// A list of validators.
        /// </returns>
        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context,
                                                                     IEnumerable<Attribute> attributes)
        {
            var items = attributes.ToList();
            if (AddImplicitRequiredAttributeForValueTypes && metadata.IsRequired &&
                !items.Any(a => a is RequiredAttribute))
                items.Add(new RequiredAttribute());



            var validators = new List<ModelValidator>();
            foreach (var attr in items.OfType<ValidationAttribute>())
            {
                // custom message, use the default localization
                if (attr.ErrorMessageResourceName != null && attr.ErrorMessageResourceType != null)
                {
                    validators.Add(new DataAnnotationsModelValidator(metadata, context, attr));
                    continue;
                }

                // specified a message, do nothing
                if (attr.ErrorMessage != null && attr.ErrorMessage != WorkaroundMarker)
                {
                    validators.Add(new DataAnnotationsModelValidator(metadata, context, attr));
                    continue;
                }


                var ctx = new GetMessageContext(attr, metadata.ContainerType, metadata.PropertyName,
                                                Thread.CurrentThread.CurrentUICulture);
                var errorMessage = ValidationMessageProviders.GetMessage(ctx);
                var formattedError = errorMessage == null
                                         ? GetMissingTranslationMessage(metadata, attr)
                                         : FormatErrorMessage(metadata, attr, errorMessage);

                var clientRules = GetClientRules(metadata, context, attr,
                                                 formattedError);
                validators.Add(new MyValidator(attr, formattedError, metadata, context, clientRules));
            }


            if (metadata.Model is IValidatableObject)
                validators.Add(new Griffin.MvcContrib.Localization.ValidatableObjectAdapter(metadata, context));


            return validators;
        }


        /// <summary>
        /// Get default message if the localized string is missing
        /// </summary>
        /// <param name="metadata">Model meta data</param>
        /// <param name="attr">Attribute to translate</param>
        /// <returns>Formatted message</returns>
        protected virtual string GetMissingTranslationMessage(ModelMetadata metadata, ValidationAttribute attr)
        {
            _logger.Warning("Failed to find translation for " + attr.GetType().Name + " on " +
                            metadata.ContainerType + "." + metadata.PropertyName);


            return string.Format("[{0}: {1}]", CultureInfo.CurrentUICulture.Name,
                                 attr.GetType().Name.Replace("Attribute", ""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadata">Model meta data</param>
        /// <param name="attr">Attribute to localize</param>
        /// <param name="errorMessage">Localized message with <c>{}</c> formatters.</param>
        /// <returns>Formatted message (<c>{}</c> has been replaced with values)</returns>
        protected virtual string FormatErrorMessage(ModelMetadata metadata, ValidationAttribute attr,
                                                    string errorMessage)
        {
            string formattedError;
            try
            {
                lock (attr)
                {
                    attr.ErrorMessage = errorMessage;
                    formattedError = attr.FormatErrorMessage(metadata.GetDisplayName());
                    attr.ErrorMessage = WorkaroundMarker;
                }
            }
            catch (Exception err)
            {
                formattedError = err.Message;
            }
            return formattedError;
        }

        /// <summary>
        /// Get client rules
        /// </summary>
        /// <param name="metadata">Model meta data</param>
        /// <param name="context">Controller context</param>
        /// <param name="attr">Attribute being localized</param>
        /// <param name="formattedError">Localized error message</param>
        /// <returns>Collection (may be empty) with error messages for client side</returns>
        protected virtual IEnumerable<ModelClientValidationRule> GetClientRules(ModelMetadata metadata,
                                                                                ControllerContext context,
                                                                                ValidationAttribute attr,
                                                                                string formattedError)
        {
            var clientValidable = attr as IClientValidatable;
            var clientRules = clientValidable == null
                                  ? _adapterFactory.Create(attr, formattedError)
                                  : clientValidable.GetClientValidationRules(
                                      metadata, context);

            foreach (var clientRule in clientRules)
            {
                clientRule.ErrorMessage = formattedError;
            }

            return clientRules;
        }

        #region Nested type: MyValidator

        private class MyValidator : ModelValidator
        {
            private readonly ValidationAttribute _attribute;
            private readonly IEnumerable<ModelClientValidationRule> _clientRules;
            private readonly string _errorMsg;

            public MyValidator(ValidationAttribute attribute, string errorMsg, ModelMetadata metadata,
                               ControllerContext controllerContext, IEnumerable<ModelClientValidationRule> clientRules)
                : base(metadata, controllerContext)
            {
                _attribute = attribute;
                _errorMsg = errorMsg;
                _clientRules = clientRules;
            }

            public override bool IsRequired
            {
                get { return _attribute is RequiredAttribute; }
            }

            public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
            {

                return _clientRules;
            }

            public override IEnumerable<ModelValidationResult> Validate(object container)
            {
                var context = new ValidationContext(container, null, null);
                var result = _attribute.GetValidationResult(Metadata.Model, context);
                if (result == null)
                    yield break;

                //if (_attribute.IsValid(Metadata.Model))
                //  yield break;

                string errorMsg;
                lock (_attribute)
                {
                    _attribute.ErrorMessage = _errorMsg;
                    errorMsg = _attribute.FormatErrorMessage(Metadata.GetDisplayName());
                    _attribute.ErrorMessage = WorkaroundMarker;
                }
                yield return new ModelValidationResult { Message = errorMsg };
            }
        }

        #endregion
    }
}