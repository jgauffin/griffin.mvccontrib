using System;
using System.Globalization;
using Griffin.MvcContrib.Localization.Types;

namespace Griffin.MvcContrib.RavenDb.Localization
{
    /// <summary>
    /// Document used to store localization entry in RavenDb.
    /// </summary>
    public class TypePrompt
    {
        public TypePrompt()
        {
        }

        public TypePrompt(string key, Type modelType, string propertyName, CultureInfo culture)
        {
            TextKey = key;
            AssemblyQualifiedName = modelType.AssemblyQualifiedName;
            FullTypeName = modelType.FullName;
            TypeName = modelType.Name;
            TextName = propertyName;
            LocaleId = culture.LCID;
        }

        public TypePrompt(CultureInfo culture, TextPrompt prompt)
        {
            AssemblyQualifiedName = prompt.Subject.Assembly.GetName().Name;
            FullTypeName = prompt.Subject.FullName;
            TypeName = prompt.Subject.Name;
            TextName = prompt.TextName;
            TextKey = prompt.Key.ToString();
            LocaleId = culture.LCID;
            UpdatedAt = prompt.UpdatedAt;
            UpdatedBy = prompt.UpdatedBy;
            Text = prompt.TranslatedText;
        }

        public TypePrompt(CultureInfo culture, TypePrompt prompt)
        {
            AssemblyQualifiedName = prompt.AssemblyQualifiedName;
            FullTypeName = prompt.FullTypeName;
            TypeName = prompt.TypeName;
            TextName = prompt.TextName;
            TextKey = prompt.TextKey;
            LocaleId = culture.LCID;
            UpdatedAt = prompt.UpdatedAt;
            UpdatedBy = prompt.UpdatedBy;
            Text = "";
        }

        public string AssemblyQualifiedName { get; set; }

        public string FullTypeName { get; set; }

        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public int LocaleId { get; set; }
        public string TextKey { get; set; }
        public string TextName { get; set; }
        public string TypeName { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return TypeName + "." + TextName + ": " + Text;
        }
    }
}