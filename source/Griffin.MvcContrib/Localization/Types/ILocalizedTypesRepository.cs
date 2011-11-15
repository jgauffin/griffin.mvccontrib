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
		/// <param name="defaultCulture">Culture used as template to be able to include all non-translated prompts</param>
		/// <returns>Collection of translations</returns>
		IEnumerable<TextPrompt> GetPrompts(CultureInfo cultureInfo, CultureInfo defaultCulture = null);

		/// <summary>
		/// Get a specific prompt
		/// </summary>
		/// <param name="culture">Culture to get prompt for</param>
		/// <param name="key">Unique key, in the specified language only, for the prompt to get)</param>
		/// <returns>Prompt if found; otherwise <c>null</c>.</returns>
		TextPrompt GetPrompt(CultureInfo culture, string key);

		/// <summary>
		/// Update translation
		/// </summary>
		/// <param name="culture">Culture that the prompt is for</param>
		/// <param name="key">Unique key, in the specified language only, for the prompt to get)</param>
		/// <param name="translatedText">Translated text string</param>
		void Update(CultureInfo culture, string key, string translatedText);

		/// <summary>
		/// Get all languages that got partial or full translations.
		/// </summary>
		/// <returns>Cultures corresponding to the translations</returns>
		IEnumerable<CultureInfo> GetAvailableLanguages();
	}
}
