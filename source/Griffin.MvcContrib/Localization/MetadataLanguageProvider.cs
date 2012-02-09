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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Griffin.MvcContrib.Localization.Types;

namespace Griffin.MvcContrib.Localization
{
    /// <summary>
    /// Just returns specified meta data
    /// </summary>
    /// <remarks>Is used when nothing else have been configured.</remarks>
    public class MetadataLanguageProvider : ILocalizedStringProvider
    {
        /// <summary>
        /// Get a localized string for a model property
        /// </summary>
        /// <param name="model">Model being localized</param>
        /// <param name="propertyName">Property to get string for</param>
        /// <returns>Translated string</returns>
        public string GetModelString(Type model, string propertyName)
        {
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, model, propertyName);
            return metadata.DisplayName;
        }

        /// <summary>
        /// Get a localized metadata for a model property
        /// </summary>
        /// <param name="model">Model being localized</param>
        /// <param name="propertyName">Property to get string for</param>
        /// <param name="metadataName">Valid names are: Watermark, Description, NullDisplayText, ShortDisplayText.</param>
        /// <returns>Translated string</returns>
        /// <remarks>
        /// Look at <see cref="ModelMetadata"/> to know more about the meta data
        /// </remarks>
        public string GetModelString(Type model, string propertyName, string metadataName)
        {
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, model, propertyName);
            return metadata.GetType().GetProperty(metadataName).GetValue(metadata, null).ToString();

        }

        /// <summary>
        /// Get a translated string for a validation attribute
        /// </summary>
        /// <param name="attributeType">Type of attribute</param>
        /// <returns>Localized validation message</returns>
        /// <remarks>
        /// Used to get localized error messages for the DataAnnotation attributes. The returned string 
        /// should have the same format as the built in messages, such as "{0} is required.".
        /// </remarks>
        public string GetValidationString(Type attributeType)
        {
            var attribute = (ValidationAttribute) Activator.CreateInstance(attributeType);
            return attribute.ErrorMessage;
        }

        /// <summary>
        /// Get a translated string for a validation attribute
        /// </summary>
        /// <param name="attributeType">Type of attribute</param>
        /// <param name="modelType">Your view model</param>
        /// <param name="propertyName">Property in your view model</param>
        /// <returns>Translated validation message if found; otherwise null.</returns>
        /// <remarks>
        /// Tries to get a validation string which is specific for a view model property.
        /// </remarks>
        public string GetValidationString(Type attributeType, Type modelType, string propertyName)
        {
            var property = modelType.GetProperty(propertyName);
            if (property == null)
                throw new InvalidOperationException(string.Format("Failed to find property {0} in {1}", propertyName, modelType.Name));

            var attribute = property.GetCustomAttributes(attributeType, true).Cast<ValidationAttribute>().FirstOrDefault();
            if (attribute == null)
                return null;

            return attribute.ErrorMessage;
        }

        /// <summary>
        /// Gets a enum string
        /// </summary>
        /// <param name="enumType">Type of enum</param>
        /// <param name="name">Name of the value to translation for</param>
        /// <returns>Translated name</returns>
        public string GetEnumString(Type enumType, string name)
        {
            DescriptionAttribute attribute;
            if (enumType.GetCustomAttributes(typeof(FlagsAttribute), true).Any())
            {
                var entries = name.Split(',');
                var description = new string[entries.Length];
                for (var i = 0; i < entries.Length; i++)
                {
                    var fieldInfo = enumType.GetField(entries[i].Trim());
                    attribute = (DescriptionAttribute)fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
                    description[i] = attribute != null ? attribute.Description : entries[i].Trim();
                }
                return string.Join(", ", description);
            }

            var memInfo = enumType.GetField(name);
            attribute = (DescriptionAttribute)memInfo.GetCustomAttributes(typeof (DescriptionAttribute), true).FirstOrDefault();
            return attribute == null ? name : attribute.Description;
        }
    }
}
