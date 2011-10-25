using System.Collections.Generic;
using System.Globalization;
using Griffin.MvcContrib.Localization.Types;

namespace Griffin.MvcContrib.Localization
{
	
	/// <summary>
	/// Repository used to provide localization strings
	/// </summary>
	public interface ILocalizedStringRepository
	{
		TextPromptCollection GetPrompts(CultureInfo cultureInfo);

		TextPrompt GetPrompt(CultureInfo culture, string key);
		void Update(CultureInfo culture, string textKey, string translatedText);
		IEnumerable<CultureInfo> GetAvailableLanguages();
	}
}
