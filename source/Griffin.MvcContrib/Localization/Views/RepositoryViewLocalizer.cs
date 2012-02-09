using System;
using System.Globalization;
using System.Web.Routing;

namespace Griffin.MvcContrib.Localization.Views
{
    /// <summary>
    ///   Uses a IViewLocalizationRepository to localize views
    /// </summary>
    /// <remarks>
    ///   Create a class and implement <see cref="IViewLocalizationRepository" /> and register it in your container to use an own repository for the view localization.
    /// </remarks>
    public class RepositoryViewLocalizer : IViewLocalizer
    {
        private readonly IViewLocalizationRepository _repository;

        /// <summary>
        ///   Initializes a new instance of the <see cref="RepositoryViewLocalizer" /> class.
        /// </summary>
        public RepositoryViewLocalizer(IViewLocalizationRepository repository)
        {
            _repository = repository;
        }

        #region IViewLocalizer Members

        /// <summary>
        ///   Translate a text prompt
        /// </summary>
        /// <param name="routeData"> Used to lookup the controller location </param>
        /// <param name="text"> Text to translate </param>
        /// <returns> </returns>
        public virtual string Translate(RouteData routeData, string text)
        {
            if (routeData == null) throw new ArgumentNullException("routeData");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            if (!_repository.Exists(CultureInfo.CurrentUICulture))
            {
                //use english as default
                var phrases = _repository.GetAllPrompts(CultureInfo.CurrentUICulture, DefaultCulture.Value,
                                                        new SearchFilter());
                foreach (var phrase in phrases)
                {
                    _repository.Save(CultureInfo.CurrentUICulture, phrase.ViewPath, phrase.TextName,
                                     phrase.TranslatedText);
                }
            }

            var textToSay = "";
            var uri = ViewPromptKey.GetViewPath(routeData);
            var id = new ViewPromptKey(uri, text);
            var prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, id);
            if (prompt == null)
                _repository.CreatePrompt(CultureInfo.CurrentUICulture, uri, text, "");
            else
                textToSay = prompt.TranslatedText;

            if (string.IsNullOrEmpty(textToSay))
                textToSay = DefaultCulture.Is(CultureInfo.CurrentUICulture)
                                ? text
                                : string.Format("{1}:[{0}]", text, CultureInfo.CurrentUICulture);

            return textToSay;
        }

        #endregion
    }
}