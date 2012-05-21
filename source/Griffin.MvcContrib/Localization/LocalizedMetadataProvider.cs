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
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Griffin.MvcContrib.Localization.Types;

namespace Griffin.MvcContrib.Localization
{
    /// <summary>
    /// Metadata provider used to localize models and their meta data.
    /// </summary>
    /// <remarks>
    /// <para>Check the namespace documentation for an example on how to use the provider.</para>
    /// </remarks>
    public class LocalizedModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        private ILocalizedStringProvider _stringProviderDontUseDirectly;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedModelMetadataProvider"/> class.
        /// </summary>
        /// <param name="stringProvider">The string provider.</param>
        public LocalizedModelMetadataProvider(ILocalizedStringProvider stringProvider)
        {
            if (stringProvider == null) throw new ArgumentNullException("stringProvider");
            _stringProviderDontUseDirectly = stringProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedModelMetadataProvider"/> class.
        /// </summary>
        /// <remarks>you need to register <see cref="ILocalizedStringProvider"/> in your IoC container.</remarks>
        public LocalizedModelMetadataProvider()
        {
        }

        private ILocalizedStringProvider Provider
        {
            get
            {
                if (_stringProviderDontUseDirectly != null)
                    return _stringProviderDontUseDirectly;


                // ASP.NET doesn't honor the IoC scope of the provider
                // which means that we can't set the dependency one,
                // but need to resolve it per request
                var provider = HttpContext.Current.Items["ILocalizedStringProvider"] as ILocalizedStringProvider;
                if (provider == null)
                {
                    provider = DependencyResolver.Current.GetService<ILocalizedStringProvider>();
                    HttpContext.Current.Items["ILocalizedStringProvider"] = provider;
                }

                if (provider == null)
                    throw new InvalidOperationException(
                        "Failed to find an 'ILocalizedStringProvider' implementation. Either include one in the LocalizedModelMetadataProvider constructor, or register an implementation in your Inversion Of Control container.");

                return provider;
            }
        }

        /// <summary>
        /// Gets the metadata for the specified property.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <param name="containerType">The type of the container.</param>
        /// <param name="modelAccessor">The model accessor.</param>
        /// <param name="modelType">The type of the model.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>
        /// The metadata for the property.
        /// </returns>
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType,
                                                        Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
            if (containerType == null || propertyName == null)
                return metadata;

            if (metadata.DisplayName == null)
            {
                metadata.DisplayName = Translate(containerType, propertyName);
                if (!DefaultUICulture.IsActive && metadata.DisplayName == null)
                    metadata.DisplayName = string.Format("[{0}: {1}]", CultureInfo.CurrentUICulture.Name, propertyName);
            }

            if (metadata.Watermark == null)
                metadata.Watermark = Translate(containerType, propertyName, "Watermark");

            if (metadata.Description == null)
                metadata.Description = Translate(containerType, propertyName, "Description");

            if (metadata.NullDisplayText == null)
                metadata.NullDisplayText = Translate(containerType, propertyName, "NullDisplayText");

            if (metadata.ShortDisplayName == null)
                metadata.ShortDisplayName = Translate(containerType, propertyName, "ShortDisplayName");

            return metadata;
        }

        /// <summary>
        /// Translate a string
        /// </summary>
        /// <param name="type">mode type</param>
        /// <param name="propertyName">Property name to translate</param>
        /// <returns>Translated string</returns>
        protected virtual string Translate(Type type, string propertyName)
        {
            return Provider.GetModelString(type, propertyName);
        }

        /// <summary>
        /// Translate a string
        /// </summary>
        /// <param name="type">Model type</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="metadataName">Meta data name</param>
        /// <returns>Translated string</returns>
        protected virtual string Translate(Type type, string propertyName, string metadataName)
        {
            return Provider.GetModelString(type, propertyName, metadataName);
        }
    }
}