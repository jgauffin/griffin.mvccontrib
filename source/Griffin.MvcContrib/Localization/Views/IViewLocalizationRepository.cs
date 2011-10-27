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
		/// <returns>A collection of prompts</returns>
		IEnumerable<TextPrompt> GetAllPrompts(CultureInfo culture);

		/// <summary>
		/// Get all languages that have translations
		/// </summary>
		/// <returns>Collection of languages</returns>
		IEnumerable<CultureInfo> GetAvailableLanguages();
			
		/// <summary>
		/// Get all prompts that have not been translated
		/// </summary>
		/// <param name="culture">Culture to get translation for</param>
		/// <param name="defaultLanguage">Default language</param>
		/// <returns>A collection of prompts</returns>
		/// <remarks>
		/// Default language will typically have more translated prompts than any other language
		/// and is therefore used to detect missing prompts.
		/// </remarks>
		IEnumerable<TextPrompt> GetNotLocalizedPrompts(CultureInfo culture, CultureInfo defaultLanguage);

		/// <summary>
		/// Create a new language
		/// </summary>
		/// <param name="culture">Language to create</param>
		/// <param name="sourceLanguage">Language to use as a template</param>
		/// <remarks>
		/// Will add empty entries for all known entries. Entries are added automatically to the default language when views
		/// are visited. This is NOT done for any other language.
		/// </remarks>
		void CreateForLanguage(CultureInfo culture, CultureInfo sourceLanguage);

		/// <summary>
		/// Get a text using it's name.
		/// </summary>
		/// <param name="culture">Culture to get prompt for</param>
		/// <param name="id">Id of the prompt</param>
		/// <returns>Prompt if found; otherwise null.</returns>
		TextPrompt GetPrompt(CultureInfo culture, string id);

		/// <summary>
		/// Save/Update a text prompt
		/// </summary>
		/// <param name="prompt">Prompt to update</param>
		void Save(TextPrompt prompt);

		bool Exists(CultureInfo cultureInfo);

		/// <summary>
		/// Create a new prompt in the specified language
		/// </summary>
		/// <param name="culture">Language that the translation is for</param>
		/// <param name="source">Prompt to use as source</param>
		/// <param name="translatedText">Translated text</param>
		void CreatePrompt(CultureInfo culture, TextPrompt source, string translatedText);
	}
}