using System;
using System.Runtime.Serialization;

namespace Griffin.MvcContrib.Localization.Views
{
    /// <summary>
    /// A text that should be translated
    /// </summary>
    [Serializable]
    [DataContract]
    public class ViewPrompt : IEquatable<ViewPrompt>
    {
        private ViewPromptKey _textKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPrompt"/> class.
        /// </summary>
        public ViewPrompt()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPrompt"/> class.
        /// </summary>
        /// <param name="localeId">New language</param>
        /// <param name="source">The source.</param>
        public ViewPrompt(int localeId, ViewPrompt source)
        {
            Key = source.Key;
            LocaleId = localeId;
            ViewPath = source.ViewPath;
            TextName = source.TextName;
            TranslatedText = "";
        }

        /// <summary>
        /// Gets or sets locale id (refer to MSDN)
        /// </summary>
        [DataMember]
        public int LocaleId { get; set; }

        /// <summary>
        /// Gets or sets view path, typically <see cref="Uri.AbsolutePath"/>
        /// </summary>
        [DataMember]
        public string ViewPath { get; set; }


        /// <summary>
        /// Gets or sets the text to translate
        /// </summary>
        [DataMember]
        public string TextName { get; set; }

        /// <summary>
        /// Gets or sets translated text
        /// </summary>
        /// <value>Empty string if not translated</value>
        [DataMember]
        public string TranslatedText { get; set; }

        /// <summary>
        /// Gets a key for the prompt.
        /// </summary>
        /// <remarks>The key must be unique in the current language but should be the
        /// same for all different languages.</remarks>
        [DataMember]
        public ViewPromptKey Key
        {
            get
            {
                if (_textKey == null)
                {
                }
                return _textKey;
            }
            set { _textKey = value; }
        }

        #region Implementation of IEquatable<ViewPrompt>

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(ViewPrompt other)
        {
            return other != null && other.Key.Equals(Key);
        }

        #endregion
    }
}