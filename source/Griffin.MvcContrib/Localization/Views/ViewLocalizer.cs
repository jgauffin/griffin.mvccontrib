using System;
using System.Globalization;
using System.Web.Routing;

namespace Griffin.MvcContrib.Localization.Views
{
    /// <summary>
    ///   Uses a <see cref="IViewLocalizationRepository" /> to localize views.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Localizes views for the <see cref="GriffinWebViewPage"/> class. Will also create empty prompts for
    /// all cultures with missing prompts.
    /// </para>
    /// </remarks>
    public class ViewLocalizer
    {
        private readonly IViewLocalizationRepository _repository;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ViewLocalizer" /> class.
        /// </summary>
        public ViewLocalizer(IViewLocalizationRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");

            _repository = repository;
        }

        /// <summary>
        /// Translate a text prompt
        /// </summary>
        /// <param name="routeData">Used to lookup the view location</param>
        /// <param name="text">Text to translate</param>
        /// <returns>
        /// String if found; otherwise null.
        /// </returns>
        public virtual string Translate(RouteData routeData, string text)
        {
            if (routeData == null) throw new ArgumentNullException("routeData");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            if (!_repository.Exists(CultureInfo.CurrentUICulture))
            {
                //use english as default
                CloneDefaultCulture();
            }

            var textToSay = "";
            var uri = ViewPromptKey.GetViewPath(routeData);
            var id = new ViewPromptKey(uri, text);
            var prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, id);
            if (prompt == null)
            {
                textToSay = LoadCommonPrompt(text);
                if (textToSay == null)
                    _repository.CreatePrompt(CultureInfo.CurrentUICulture, uri, text, "");
            }
            else
                textToSay = prompt.TranslatedText;

            return string.IsNullOrEmpty(textToSay)
                       ? FormatMissingPrompt(text)
                       : textToSay;
        }

        /// <summary>
        /// Format output for a missing prompt
        /// </summary>
        /// <param name="text">View text</param>
        /// <returns>Plain text if default culture; otherwise culture tagged text.</returns>
        /// <example>
        /// Default culture:
        /// <code>
        /// localizer.FormatMissingPrompt("Hello world"); // --> hello world
        /// </code>
        /// Another culture:
        /// <code>
        /// localizer.FormatMissingPrompt("Hello world"); // --> [sv-se: hello world]
        /// </code>
        /// </example>
        protected virtual string FormatMissingPrompt(string text)
        {
            return DefaultUICulture.Is(CultureInfo.CurrentUICulture)
                       ? text
                       : string.Format("{1}:[{0}]", text, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Load a common prompt
        /// </summary>
        /// <param name="text">Text to translate</param>
        /// <returns>Translation if found; otherwise null</returns>
        /// <remarks>Used to avoid duplications of prompts.</remarks>
        protected virtual string LoadCommonPrompt(string text)
        {
            var key = new ViewPromptKey("CommonPrompts", text);
            var prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, key);
            return prompt == null ? null : prompt.TranslatedText;
        }

        /// <summary>
        /// Clone the default culture 
        /// </summary>
        /// <remarks>A translation for the current UI culture is missing. Clones the default culture into the current one</remarks>
        /// <seealso cref="DefaultUICulture" />
        protected virtual void CloneDefaultCulture()
        {
            var phrases = _repository.GetAllPrompts(CultureInfo.CurrentUICulture, DefaultUICulture.Value,
                                                    new SearchFilter());
            foreach (var phrase in phrases)
            {
                _repository.Save(CultureInfo.CurrentUICulture, phrase.ViewPath, phrase.TextName,
                                 phrase.TranslatedText);
            }
        }
    }
}