using System;
using System.Globalization;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Localization.Types
{
    /// <summary>
    /// Uses a <see cref="ILocalizedTypesRepository"/> to find all strings
    /// </summary>
    /// <remarks>
    /// <para>
    /// Set the <see cref="DefaultUICulture"/>. It's used to determine if the provider should indicate that
    /// a translation is missing.
    /// </para>
    /// <para>
    /// Use <see cref="CommonPrompts"/> as the requested type to handle translations that are common for many objects.
    /// </para>
    /// </remarks>
    public class RepositoryStringProvider : ILocalizedStringProvider
    {
        private readonly ILocalizedTypesRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryStringProvider"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public RepositoryStringProvider(ILocalizedTypesRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
        }

        #region ILocalizedStringProvider Members

        /// <summary>
        /// Get a localized string for a model property
        /// </summary>
        /// <param name="model">Model being localized</param>
        /// <param name="propertyName">Property to get string for</param>
        /// <returns>Translated string if found; otherwise null.</returns>
        public string GetModelString(Type model, string propertyName)
        {
            return Translate(model, propertyName);
        }

        /// <summary>
        /// Get a localized metadata for a model property
        /// </summary>
        /// <param name="model">Model being localized</param>
        /// <param name="propertyName">Property to get string for</param>
        /// <param name="metadataName">Valid names are: Watermark, Description, NullDisplayText, ShortDisplayText.</param>
        /// <returns>Translated string if found; otherwise null.</returns>
        /// <remarks>
        /// Look at <see cref="ModelMetadata"/> to know more about the meta data
        /// </remarks>
        public string GetModelString(Type model, string propertyName, string metadataName)
        {
            return Translate(model, propertyName + "_" + metadataName);
        }

        /// <summary>
        /// Get a translated string for a validation attribute
        /// </summary>
        /// <param name="attributeType">Type of attribute</param>
        /// <returns>Translated validtion message if found; otherwise null.</returns>
        /// <remarks>
        /// Used to get localized error messages for the DataAnnotation attributes. The returned string 
        /// should have the same format as the built in messages, such as "{0} is required.".
        /// </remarks>
        public string GetValidationString(Type attributeType)
        {
            return Translate(attributeType, "class");
        }

        /// <summary>
        /// Get a translated string for a validation attribute
        /// </summary>
        /// <param name="attributeType">Type of attribute</param>
        /// <param name="modelType">Your view model</param>
        /// <param name="propertyName">Property in your view model</param>
        /// <returns>
        /// Translated validation message if found; otherwise null.
        /// </returns>
        public string GetValidationString(Type attributeType, Type modelType, string propertyName)
        {
            return Translate(modelType, propertyName + "_" + attributeType.Name);
        }

        /// <summary>
        /// Gets a enum string
        /// </summary>
        /// <param name="enumType">Type of enum</param>
        /// <param name="name">Name of the value to translation for</param>
        /// <returns>Translated name if found; otherwise null.</returns>
        public string GetEnumString(Type enumType, string name)
        {
            return Translate(enumType, name);
        }

        #endregion

        private string Translate(Type type, string name)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (name == null) throw new ArgumentNullException("name");


            if (!string.IsNullOrEmpty(type.Namespace) && type.Namespace.StartsWith("Griffin.MvcContrib"))
            {
                return null;
            }


            var key = new TypePromptKey(type, name);
            var prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, key) ??
                         _repository.GetPrompt(CultureInfo.CurrentUICulture,
                                               new TypePromptKey(typeof (CommonPrompts), name));


            if (prompt == null)
            {
                _repository.Save(CultureInfo.CurrentUICulture, type, name, "");
            }
            else
            {
                if (!string.IsNullOrEmpty(prompt.TranslatedText))
                    return prompt.TranslatedText;
            }

            return name.EndsWith("NullDisplayText") ? "" : null;
        }
    }
}