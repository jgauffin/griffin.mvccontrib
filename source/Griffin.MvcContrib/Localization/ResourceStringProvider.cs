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
using System.Linq;
using System.Resources;
using System.Web.Mvc;
using Griffin.MvcContrib.Localization.Types;

namespace Griffin.MvcContrib.Localization
{
    /// <summary>
    /// Used to return strings from one or more StringTables.
    /// </summary>
    /// <example>
    /// <code>
    /// var provider = new ResourceStringProvider(MyLocalizedStrings.ResourceProvider);
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>Model translations should have the following format: "ClassName_PropertyName", for example: "User_FirstName". All
    /// extra metadata should have the following format: "ClassName_PropertyName_MetadataName".</para>
    /// <para>
    /// Validation error messages should just be named as the attributes, but without the "Attribute" suffix. Example: "Required".
    /// </para>
    /// </remarks>
    public class ResourceStringProvider : ILocalizedStringProvider
    {
        private readonly List<ResourceManager> _resourceManagers = new List<ResourceManager>();


        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStringProvider"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        /// <example>
        /// </example>
        public ResourceStringProvider(params ResourceManager[] resourceManager)
        {
            _resourceManagers.AddRange(resourceManager);
        }

        #region ILocalizedStringProvider Members

        /// <summary>
        /// Get a localized string for a model property
        /// </summary>
        /// <param name="model">Model being localized</param>
        /// <param name="propertyName">Property to get string for</param>
        /// <returns>Translated string</returns>
        public string GetModelString(Type model, string propertyName)
        {
            return GetString(Format(model, propertyName));
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
            return GetString(Format(model, propertyName, metadataName));
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
            var name = Format(attributeType);
            return GetString(name);
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
            var name = Format(modelType, propertyName, attributeType.Name.Replace("Attribute", ""));
            return GetString(name);
        }

        /// <summary>
        /// Gets a enum string
        /// </summary>
        /// <param name="enumType">Type of enum</param>
        /// <param name="name">Name of the value to translation for</param>
        /// <returns>Translated name</returns>
        /// <remarks>enums has the same format as models: EnumTypeName_ValueName</remarks>
        public string GetEnumString(Type enumType, string name)
        {
            return GetString(Format(enumType, name));
        }

        #endregion

        /// <summary>
        /// Format the model informaiton into a StringTable key.
        /// </summary>
        /// <param name="type">Model type</param>
        /// <param name="propertyName">Name of the property in the model</param>
        /// <param name="extras">Extras used during formatting</param>
        /// <returns>String Table key</returns>
        protected virtual string Format(Type type, string propertyName, params string[] extras)
        {
            var baseStr = string.Format("{0}_{1}", type.Name, propertyName);
            return extras.Aggregate(baseStr, (current, extra) => current + ("_" + extra));
        }

        /// <summary>
        /// Format the attribute type information into a StringTable key
        /// </summary>
        /// <param name="attributeType">Attribute type</param>
        /// <returns>String Table key</returns>
        protected virtual string Format(Type attributeType)
        {
            return attributeType.Name.Replace("Attribute", "");
        }

        /// <summary>
        /// Get a string from one of the string tables.
        /// </summary>
        /// <param name="name">String table item key</param>
        /// <returns>string if found; otherwise null.</returns>
        private string GetString(string name)
        {
            var result =  _resourceManagers.Select(resourceManager => resourceManager.GetString(name)).FirstOrDefault(value => value != null);
            return result;
        }
    }
}