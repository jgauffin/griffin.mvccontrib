using System;
using System.Globalization;
using Griffin.MvcContrib.Localization.Types;

namespace Griffin.MvcContrib.RavenDb.Localization
{
    /// <summary>
    /// Document used to store localization entry in RavenDb.
    /// </summary>
    public class TypePromptDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypePromptDocument"/> class.
        /// </summary>
        public TypePromptDocument()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypePromptDocument"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="culture">The culture.</param>
        public TypePromptDocument(TypePromptKey key, Type modelType, string propertyName, CultureInfo culture)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (modelType == null) throw new ArgumentNullException("modelType");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            TextKey = key.ToString();
            FullTypeName = modelType.FullName;
            TypeName = modelType.Name;
            TextName = propertyName;
            LocaleId = culture.LCID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypePromptDocument"/> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="prompt">The prompt.</param>
        public TypePromptDocument(CultureInfo culture, TypePrompt prompt)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (prompt == null) throw new ArgumentNullException("prompt");
            FullTypeName = prompt.TypeFullName;
            TypeName = prompt.TypeName;
            TextName = prompt.TextName;
            TextKey = prompt.Key.ToString();
            LocaleId = culture.LCID;
            UpdatedAt = prompt.UpdatedAt;
            UpdatedBy = prompt.UpdatedBy;
            Text = prompt.TranslatedText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypePromptDocument"/> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="prompt">The prompt.</param>
        public TypePromptDocument(CultureInfo culture, TypePromptDocument prompt)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (prompt == null) throw new ArgumentNullException("prompt");
            FullTypeName = prompt.FullTypeName;
            TypeName = prompt.TypeName;
            TextName = prompt.TextName;
            TextKey = prompt.TextKey;
            LocaleId = culture.LCID;
            UpdatedAt = prompt.UpdatedAt;
            UpdatedBy = prompt.UpdatedBy;
            Text = "";
        }

        /// <summary>
        /// Gets or sets Type.FullName
        /// </summary>
        public string FullTypeName { get; set; }

        /// <summary>
        /// Gets or sets when the prompt was updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the user that updated the prompt
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets LCID identifier
        /// </summary>
        public int LocaleId { get; set; }

        /// <summary>
        /// Gets or sets generated key (which is unique within the language)
        /// </summary>
        public string TextKey { get; set; }

        /// <summary>
        /// Gets or sets property name
        /// </summary>
        public string TextName { get; set; }

        /// <summary>
        /// Gets or sets Type.Name
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets translation
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return TypeName + "." + TextName + ": " + Text;
        }
    }
}