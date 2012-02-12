using System.Collections.Generic;
using System.Globalization;

namespace Griffin.MvcContrib.Localization.Views
{
    /// <summary>
    /// Repository used to handle localized strings
    /// </summary>
    public interface IViewLocalizationRepository
    {
        /// <summary>
        /// Get all prompts that have been created for an language
        /// </summary>
        /// <param name="culture">Culture to get translation for</param>
        /// <param name="templateCulture">Culture to find not translated prompts in (or same culture to disable) </param>
        /// <param name="filter">Used to limit the search result</param>
        /// <returns>A collection of prompts</returns>
        IEnumerable<ViewPrompt> GetAllPrompts(CultureInfo culture, CultureInfo templateCulture, SearchFilter filter);

        /// <summary>
        /// Create translation for a new language
        /// </summary>
        /// <param name="culture">Language to create</param>
        /// <param name="templateCulture">Language to use as a template</param>
        void CreateLanguage(CultureInfo culture, CultureInfo templateCulture);


        /// <summary>
        /// Get all languages that have translations
        /// </summary>
        /// <returns>Collection of languages</returns>
        IEnumerable<CultureInfo> GetAvailableLanguages();

        /// <summary>
        /// Get a text using it's name.
        /// </summary>
        /// <param name="culture">Culture to get prompt for</param>
        /// <param name="key"> </param>
        /// <returns>Prompt if found; otherwise null.</returns>
        ViewPrompt GetPrompt(CultureInfo culture, ViewPromptKey key);

        /// <summary>
        /// Save/Update a text prompt
        /// </summary>
        /// <param name="culture">Language to save prompt in</param>
        /// <param name="viewPath">Path to view. You can use <see cref="ViewPromptKey.GetViewPath"/></param>
        /// <param name="textName">Text to translate</param>
        /// <param name="translatedText">Translated text</param>
        void Save(CultureInfo culture, string viewPath, string textName, string translatedText);

        /// <summary>
        /// checks if the specified language exists.
        /// </summary>
        /// <param name="cultureInfo">Language to find</param>
        /// <returns>true if found; otherwise false.</returns>
        bool Exists(CultureInfo cultureInfo);

        /// <summary>
        /// Create a new prompt in the specified language
        /// </summary>
        /// <param name="culture">Language that the translation is for</param>
        /// <param name="viewPath">Path to view. You can use <see cref="ViewPromptKey.GetViewPath"/></param>
        /// <param name="textName">Text to translate</param>
        /// <param name="translatedText">Translated text</param>
        void CreatePrompt(CultureInfo culture, string viewPath, string textName, string translatedText);

        /// <summary>
        /// Delete a prompt
        /// </summary>
        /// <param name="cultureInfo">Culture to delete the prompt for</param>
        /// <param name="key">Prompt key</param>
        void Delete(CultureInfo cultureInfo, ViewPromptKey key);
    }
}