using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Web.Hosting;

namespace Griffin.MvcContrib.Localization.Views
{
	/// <summary>
	/// Uses files to store translated view strings
	/// </summary>
	/// <remarks>
	/// <para>
	/// Caches all strings in memory which means that you should keep a single instance of the repository
	/// if you need performance.
	/// </para>
	/// <para>
	/// The current implementation stores the files in your appdata folder using the JSON serializer. You
	/// can switch locactions or serializer by deriving the class and implement <see cref="GetFullPath"/> or
	/// <see cref="Serialize"/> and <see cref="Deserialize"/>.
	/// </para>
	/// </remarks>
	public class ViewStringFileRepository : IViewStringsRepository
	{
		private static readonly object _writeLock = new object();

		private readonly Dictionary<CultureInfo, TextPromptCollection> _languages =
			new Dictionary<CultureInfo, TextPromptCollection>();

		#region IViewStringsRepository Members

		public IEnumerable<TextPrompt> GetAllPrompts(CultureInfo culture)
		{
			return GetLanguage(culture);
		}

		/// <summary>
		/// Get all languages that have translations
		/// </summary>
		/// <returns>Collection of languages</returns>
		public IEnumerable<CultureInfo> GetAvailableLanguages()
		{
			var fullPath = GetFullPath(new CultureInfo(1053)).ToLower().Replace("sv-se", "*");
			var files = Directory.GetFiles(Path.GetDirectoryName(fullPath), Path.GetFileName(fullPath));
			var cultures = new List<CultureInfo>();
			foreach (var file in files)
			{
				var languageCode = Path.GetExtension(Path.GetFileNameWithoutExtension(file)).TrimStart('.');
				var culture = new CultureInfo(languageCode);
				cultures.Add(culture);
			}
			return cultures;
		}

		/// <summary>
		/// Create a new language
		/// </summary>
		/// <param name="culture">Language to create</param>
		/// <param name="sourceLanguage">Language to use as a template</param>
		/// <remarks>
		/// Will add empty entries for all known entries. Entries are added automatically to the default language when views
		/// are visited. This is NOT done for any other language.
		/// </remarks>
		public void CreateForLanguage(CultureInfo culture, CultureInfo sourceLanguage)
		{
			var prompts = new TextPromptCollection(culture);
			var sourcePrompts = GetLanguage(sourceLanguage);
			prompts.AddRange(sourcePrompts.Select(p => new TextPrompt(p) {TranslatedText = ""}));

			SaveLanguage(culture, prompts);
			_languages.Add(culture, prompts);
		}

		/// <summary>
		/// Get a text using it's name.
		/// </summary>
		/// <param name="culture"></param>
		/// <param name="textName"></param>
		/// <returns></returns>
		public TextPrompt GetPrompt(CultureInfo culture, string id)
		{
			var prompts = GetLanguage(CultureInfo.CurrentUICulture);
			return prompts.Get(id);
		}

		/// <summary>
		/// Save/Update a text prompt
		/// </summary>
		/// <param name="prompt">Prompt to update</param>
		public void Save(TextPrompt prompt)
		{
			if (prompt.TextKey == null)
				throw new InvalidOperationException("Prompt id may not be null");

			var prompts = GetLanguage(CultureInfo.CurrentUICulture);

			var thePrompt = prompts.Get(prompt.TextKey);
			if (thePrompt == null)
				prompts.Add(prompt);
			else if (!ReferenceEquals(prompt, thePrompt))
				thePrompt.TranslatedText = prompt.TranslatedText;

			SaveLanguage(prompts.Culture, prompts);
		}

		public bool Exists(CultureInfo cultureInfo)
		{
			return GetLanguage(cultureInfo) != null;
		}

		/// <summary>
		/// Create a new prompt in the specified language
		/// </summary>
		/// <param name="culture">Language that the translation is for</param>
		/// <param name="source">Prompt to use as source</param>
		/// <param name="translatedText">Translated text</param>
		public void CreatePrompt(CultureInfo culture, TextPrompt source, string translatedText)
		{
			var prompt = new TextPrompt(source)
			             	{
			             		LocaleId = culture.LCID,
			             		TranslatedText = translatedText
			             	};
			var language = GetLanguage(culture) ?? CreateForLanguage(culture);
			language.Add(prompt);
			SaveLanguage(culture, language);
		}

		#endregion

		protected TextPromptCollection LoadLanguage(CultureInfo culture)
		{
			var filename = GetFullPath(culture);
			if (!File.Exists(filename))
				return null;

			using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				var prompts = new TextPromptCollection(culture);
				var items = Deserialize(stream);
				prompts.AddRange(items);
				return prompts;
			}
		}

		/// <summary>
		/// Serialize items into the specified stream 
		/// </summary>
		/// <param name="stream">Stream to serialize to</param>
		/// <param name="prompts">Prompts to serialize</param>
		protected virtual void Serialize(Stream stream, List<TextPrompt> prompts)
		{
			var serializer = new DataContractJsonSerializer(prompts.GetType());
			serializer.WriteObject(stream, prompts);
		}

		/// <summary>
		/// Deserialize items from a stream
		/// </summary>
		/// <param name="stream">Stream containing serialized items</param>
		/// <returns>Collection of items (or an empty collection)</returns>
		protected virtual IEnumerable<TextPrompt> Deserialize(Stream stream)
		{
			var serializer = new DataContractJsonSerializer(typeof(List<TextPrompt>));
			return (IEnumerable<TextPrompt>)serializer.ReadObject(stream);
		}

		private string GetFullPath(CultureInfo culture)
		{
			return HostingEnvironment.MapPath(string.Format(@"~/App_Data/ViewLocalization.{0}.dat", culture.Name));
		}

		protected TextPromptCollection GetLanguage(CultureInfo culture)
		{
			TextPromptCollection prompts;
			if (_languages.TryGetValue(culture, out prompts))
				return prompts;

			lock (_writeLock)
			{
				if (_languages.TryGetValue(culture, out prompts))
					return prompts;

				prompts = LoadLanguage(culture);
				if (prompts == null)
				{
					prompts = new TextPromptCollection(culture);
					_languages.Add(culture, prompts);
				}
			}

			return prompts;
		}

		private void SaveLanguage(CultureInfo cultureInfo, IEnumerable<TextPrompt> prompts)
		{
			lock (_writeLock)
			{
				using (var stream = new FileStream(GetFullPath(cultureInfo), FileMode.Create, FileAccess.Write, FileShare.None))
				{
					Serialize(stream, prompts.ToList());
				}
			}
		}

		public IEnumerable<TextPrompt> GetNotLocalizedPrompts(CultureInfo culture, CultureInfo defaultCulture)
		{
			var prompts = GetLanguage(culture).Where(p => string.IsNullOrEmpty(p.TranslatedText));
			var defaultPrompts = GetLanguage(defaultCulture);

			return
				defaultPrompts.Except(prompts).Select(
					source => new TextPrompt(source)
					          	{
					          		LocaleId = culture.LCID,
					          		TranslatedText = ""
					          	}).ToList();
		}

		public TextPromptCollection CreateForLanguage(CultureInfo culture)
		{
			var prompts = new TextPromptCollection(culture);
			SaveLanguage(culture, prompts);
			_languages.Add(culture, prompts);
			return prompts;
		}
	}
}