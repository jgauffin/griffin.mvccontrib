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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Logging;
using Griffin.MvcContrib.Providers;

namespace Griffin.MvcContrib.Localization
{
    /// <summary>
    /// Used to localize DataAnnotation attribute error messages and view models
    /// </summary>
    /// <remarks>
    /// <para>
    /// Check for namespace documentation for an example on how to use the provider.
    /// </para>
    /// <para>Are you missing validation rules for an attribute? Do not try to use the original validation rules. The standard attributes
    /// uses some nasty delegates to handle the error message. Screwing with them should be handled with care. You should therefore patch
    /// <see cref="ValidationAttributeAdapterFactory"/> instead of messing with something in here.
    /// </para>
    /// </remarks>
    public class LocalizedModelValidatorProvider : DataAnnotationsModelValidatorProvider, IDisposable
    {
        private readonly ValidationAttributeAdapterFactory _adapterFactory = new ValidationAttributeAdapterFactory();
        private readonly ILogger _logger = LogProvider.Current.GetLogger<LocalizedModelValidatorProvider>();
        private ILocalizedStringProvider _stringProviderDontUsedirectly;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedModelValidatorProvider"/> class.
        /// </summary>
        /// <param name="stringProvider">The string provider.</param>
        public LocalizedModelValidatorProvider(ILocalizedStringProvider stringProvider)
            : this()
        {
            _stringProviderDontUsedirectly = stringProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedModelValidatorProvider"/> class.
        /// </summary>
        /// <remarks>you need to register <see cref="ILocalizedStringProvider"/> in your IoC container or use
        /// the other constructor.</remarks>
        public LocalizedModelValidatorProvider()
        {
        }

        /// <summary>
        /// Gets provider using lazy loading and DependencyResolver
        /// </summary>
        private ILocalizedStringProvider Provider
        {
            get
            {
                if (_stringProviderDontUsedirectly != null)
                    return _stringProviderDontUsedirectly;


                // ASP.NET doesn't honor the IoC scope of the provider
                // which means that we can't set the dependency one,
                // but need to resolve it per request
                var provider = HttpContext.Current.Items["ILocalizedStringProvider"] as ILocalizedStringProvider;
                if (provider == null)
                {
                    provider = DependencyResolver.Current.GetService<ILocalizedStringProvider>();
                    HttpContext.Current.Items["ILocalizedStringProvider"] = provider;
                }
                else
                    Trace.WriteLine("** Using cached provider ");


                if (provider == null)
                    throw new InvalidOperationException(
                        "Failed to find an 'ILocalizedStringProvider' implementation. Either include one in the LocalizedModelMetadataProvider constructor, or register an implementation in your Inversion Of Control container.");

                return provider;
            }
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
                if (string.IsNullOrEmpty(attr.ErrorMessageResourceName) && string.IsNullOrEmpty(attr.ErrorMessage))
                {
                    var errorMessage =
                        Provider.GetValidationString(attr.GetType(), metadata.ContainerType, metadata.PropertyName) ??
                        Provider.GetValidationString(attr.GetType());

                    // we have not translated the attribute yet.
                    if (errorMessage == null)
                    {
                        _logger.Warning("Failed to find translation for " + attr.GetType().Name + " on " +
                                        metadata.ContainerType + "." + metadata.PropertyName);


                        if (CultureInfo.CurrentUICulture.Name.StartsWith("en"))
                            errorMessage = ValidationAttributesStringProvider.Current.GetString(attr.GetType(),
                                                                                              CultureInfo.CurrentUICulture);
                        else
                            errorMessage = string.Format("[{0}: {1}]", CultureInfo.CurrentUICulture.Name,
                                                         attr.GetType().Name.Replace("Attribute", ""));
                    }

                    string formattedError;
                    try
                    {
                        lock (attr)
                        {
                            attr.ErrorMessage = errorMessage;
                            formattedError = attr.FormatErrorMessage(metadata.GetDisplayName());
                            attr.ErrorMessage = null;
                        }
                    }
                    catch (Exception err)
                    {
                        formattedError = err.Message;
                    }

                    validators.Add(new MyValidator(attr, errorMessage, metadata, context,
                                                   _adapterFactory.Create(attr, formattedError)));
                }
                else
                {
                    var clientValidable = attr as IClientValidatable;
                    var clientRules = clientValidable == null
                                          ? new ModelClientValidationRule[0]
                                          : clientValidable.GetClientValidationRules(metadata, context);
                    validators.Add(new MyValidator(attr, attr.ErrorMessage, metadata, context, clientRules));
                }
            }

            return validators;
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
                if (_attribute.IsValid(Metadata.Model))
                    yield break;

                string errorMsg;
                lock (_attribute)
                {
                    _attribute.ErrorMessage = _errorMsg;
                    errorMsg = _attribute.FormatErrorMessage(Metadata.GetDisplayName());
                    _attribute.ErrorMessage = null;
                }
                yield return new ModelValidationResult { Message = errorMsg };
            }
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}