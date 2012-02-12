using System;
using System.Threading;
using Griffin.MvcContrib.Localization.Views;

namespace Griffin.MvcContrib.RavenDb.Localization
{
    /// <summary>
    /// A phrase which can be stored in a data source.
    /// </summary>
    public class ViewPromptDocument
    {
        /// <summary>
        /// Use as URI path for phrases that are shared between views.
        /// </summary>
        public const string CommonPhrases = "CommonPhrases";

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPromptDocument"/> class.
        /// </summary>
        public ViewPromptDocument()
        {
            UpdatedAt = DateTime.Now;
            UpdatedBy = Thread.CurrentPrincipal.Identity.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPromptDocument"/> class.
        /// </summary>
        /// <param name="prompt">The prompt to copy.</param>
        public ViewPromptDocument(ViewPrompt prompt)
        {
            if (prompt == null) throw new ArgumentNullException("prompt");

            ViewPath = prompt.ViewPath;
            UpdatedAt = DateTime.Now;
            UpdatedBy = Thread.CurrentPrincipal.Identity.Name;
            LocaleId = prompt.LocaleId;
            TextKey = prompt.Key.ToString();
            TextName = prompt.TextName;
            Text = prompt.TranslatedText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPromptDocument"/> class.
        /// </summary>
        /// <param name="prompt">The prompt to copy.</param>
        public ViewPromptDocument(ViewPromptDocument prompt)
        {
            if (prompt == null) throw new ArgumentNullException("prompt");

            ViewPath = prompt.ViewPath;
            TextName = prompt.TextName;
            TextKey = prompt.TextKey;
            LocaleId = prompt.LocaleId;
            UpdatedAt = prompt.UpdatedAt;
            UpdatedBy = prompt.UpdatedBy;
            Text = prompt.Text;
        }

        /// <summary>
        /// Gets or sets when this prompt was updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets identity of the user that updated the prompt
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets LCID
        /// </summary>
        public int LocaleId { get; set; }

        /// <summary>
        /// Gets or sets identifier (same of this prompt in all languages)
        /// </summary>
        /// <seealso cref="ViewPromptKey"/>
        public string TextKey { get; set; }

        /// <summary>
        /// Gets or sets source text
        /// </summary>
        public string TextName { get; set; }

        /// <summary>
        /// Gets or sets view path
        /// </summary>
        public string ViewPath { get; set; }

        /// <summary>
        /// Gets or sets translated text
        /// </summary>
        public string Text { get; set; }
    }
}