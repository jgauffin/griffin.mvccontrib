using System.Globalization;

namespace Griffin.MvcContrib.Areas.Griffin.Models.LocalizeTypes
{
    public class TypePrompt
    {
        private readonly Localization.Types.TypePrompt _prompt;

        public TypePrompt()
        {
        }


        public TypePrompt(Localization.Types.TypePrompt prompt)
        {
            _prompt = prompt;
        }

        public string TextKey
        {
            get { return _prompt.Key.ToString(); }
        }

        /// <summary>
        /// Gets or sets locale id (refer to MSDN)
        /// </summary>
        public int LocaleId
        {
            get { return _prompt.LocaleId; }
        }

        /// <summary>
        /// Gets or sets controller that the text is for
        /// </summary>
        public string TypeName
        {
            get { return _prompt.Subject.FullName; }
        }

        public string FullTypeName
        {
            get { return _prompt.Subject.AssemblyQualifiedName; }
        }

        public string ModelName
        {
            get { return _prompt.Subject.Name; }
        }

        public string Namespace
        {
            get { return _prompt.Subject.Namespace; }
        }

        /// <summary>
        /// Gets or sets view name (unique in combination with controller name=
        /// </summary>
        public string TextName
        {
            get { return _prompt.TextName; }
        }


        /// <summary>
        /// Gets or sets translated text
        /// </summary>
        /// <value>Empty string if not translated</value>
        public string TranslatedText
        {
            get { return _prompt.TranslatedText; }
        }

        public CultureInfo Culture
        {
            get { return new CultureInfo(_prompt.LocaleId); }
        }
    }
}