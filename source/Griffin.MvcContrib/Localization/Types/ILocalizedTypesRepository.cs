using System;
using System.Collections.Generic;
using System.Globalization;

namespace Griffin.MvcContrib.Localization.Types
{
    /// <summary>
    /// Repository used to provide localization strings for different types such as validation attributes and models.
    /// </summary>
    public interface ILocalizedTypesRepository
    {
        /// <summary>
        /// Get all prompts
        /// </summary>
        /// <param name="cultureInfo">Culture to get prompts for</param>
        /// <param name="templateCulture">Culture used as template to be able to include all non-translated prompts</param>
        /// <param name="filter">Filter to limit the search result </param>
        /// <returns>
        /// Collection of translations
        /// </returns>
        IEnumerable<TypePrompt> GetPrompts(CultureInfo cultureInfo, CultureInfo templateCulture, SearchFilter filter);

        /// <summary>
        /// Create translation for a new language
        /// </summary>
        /// <param name="culture">Language to create</param>
        /// <param name="templateCulture">Language to use as a template</param>
        void CreateLanguage(CultureInfo culture, CultureInfo templateCulture);


        /// <summary>
        /// Get a specific prompt
        /// </summary>
        /// <param name="culture">Culture to get prompt for</param>
        /// <param name="key">Key which is unique in the current language</param>
        /// <returns>Prompt if found; otherwise <c>null</c>.</returns>
        TypePrompt GetPrompt(CultureInfo culture, TypePromptKey key);

        /// <summary>
        /// Create  or update a prompt
        /// </summary>
        /// <param name="culture">Culture that the prompt is for</param>
        /// <param name="type">Type being localized</param>
        /// <param name="name">Property name and any additonal names (such as metadata name, use underscore as delimiter)</param>
        /// <param name="translatedText">Translated text string</param>
        void Save(CultureInfo culture, Type type, string name, string translatedText);

        /// <summary>
        /// Get all languages that got partial or full translations.
        /// </summary>
        /// <returns>Cultures corresponding to the translations</returns>
        IEnumerable<CultureInfo> GetAvailableLanguages();

        /// <summary>
        /// Update an existing prompt
        /// </summary>
        /// <param name="cultureInfo">Culture to update prompt in</param>
        /// <param name="key">Generated key for prompt, unique in the specified language only</param>
        /// <param name="translatedText">New translated text</param>
        void Update(CultureInfo cultureInfo, TypePromptKey key, string translatedText);

        /// <summary>
        /// Delete a prompt.
        /// </summary>
        /// <param name="culture">Culture to delete prompt in</param>
        /// <param name="key">Prompt key</param>
        void Delete(CultureInfo culture, TypePromptKey key);
    }
}