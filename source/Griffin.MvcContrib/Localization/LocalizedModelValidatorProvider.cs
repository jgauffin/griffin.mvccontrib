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
using System.Linq;
using System.Web.Mvc;
using Griffin.MvcContrib.Localization.Types;

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
		private readonly ILocalizedStringProvider _stringProviderDontUsedirectly;
		private ILogger _logger = LogProvider.Current.GetLogger<LocalizedModelValidatorProvider>();

		ValidationAttributeAdapterFactory _adapterFactory = new ValidationAttributeAdapterFactory();

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalizedModelValidatorProvider"/> class.
		/// </summary>
		/// <param name="stringProvider">The string provider.</param>
		public LocalizedModelValidatorProvider(ILocalizedStringProvider stringProvider)
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

		class MyValidator : ModelValidator
		{
			private readonly ValidationAttribute _attribute;
			private readonly string _errorMsg;
			private readonly IEnumerable<ModelClientValidationRule> _create;

			public MyValidator(ValidationAttribute attribute, string errorMsg, ModelMetadata metadata, ControllerContext controllerContext, IEnumerable<ModelClientValidationRule> create)
				: base(metadata, controllerContext)
			{
				_attribute = attribute;
				_errorMsg = errorMsg;
				_create = create;
			}

			public override bool IsRequired
			{
				get
				{
					return _attribute is RequiredAttribute;
				}
			}

			public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
			{
				return _create;
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
			if (AddImplicitRequiredAttributeForValueTypes && metadata.IsRequired && !items.Any(a => a is RequiredAttribute))
				items.Add(new RequiredAttribute());

			var validators = new List<ModelValidator>();
			foreach (var attr in items.OfType<ValidationAttribute>())
			{
				if (string.IsNullOrEmpty(attr.ErrorMessageResourceName) && string.IsNullOrEmpty(attr.ErrorMessage))
				{
					var errorMessage = Provider.GetValidationString(attr.GetType());

					string formattedError = "";
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

					validators.Add(new MyValidator(attr, errorMessage, metadata, context, _adapterFactory.Create(attr, formattedError)));
				}
				else
					validators.Add(new DataAnnotationsModelValidator(metadata, context, attr));
			}

			return validators;
			// */
			/*
			var items = base.GetValidators(metadata, context);
			foreach (var attr in items.OfType<ValidationAttribute>().Where(p => string.IsNullOrEmpty(p.ErrorMessageResourceName)))
			{
				if (attr.ErrorMessage != null)
					_logger.Debug("Current message: " + attr.ErrorMessage);
				var value = Provider.GetValidationString(attr.GetType());
				_logger.Debug("Localized: " + value);
				if (!string.IsNullOrEmpty(value))
					attr.ErrorMessage = value;
			}
			return items;
			*/
		}

		protected ILocalizedStringProvider Provider
		{
			get { return _stringProviderDontUsedirectly ?? DependencyResolver.Current.GetService<ILocalizedStringProvider>(); }
		}
	}
}