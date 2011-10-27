using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Localization.Types
{

	/// <summary>
	/// Uses to localize everything for <see cref="ILocalizedStringProvider"/> in files that are placed in the AppData folder.
	/// </summary>
	public class FileStringProvider : ILocalizedStringProvider, ILocalizedTypesRepository
	{
		private static readonly object _writeLock = new object();

		private readonly Dictionary<CultureInfo, TextPromptCollection> _languages =
			new Dictionary<CultureInfo, TextPromptCollection>();

		public FileStringProvider()
		{
			DefaultCulture = new CultureInfo(1033);
		}

		public CultureInfo DefaultCulture { get; set; }

		/// <summary>
		/// Serialize items into the specified stream 
		/// </summary>
		/// <param name="stream">Stream to serialize to</param>
		/// <param name="prompts">Prompts to serialize</param>
		protected virtual void Serialize(Stream stream, List<TextPrompt> prompts)
		{
			var serializer = new DataContractJsonSerializer(typeof(List<TextPrompt>));
			serializer.WriteObject(stream, prompts);
		}

		/// <summary>
		/// Deserialize items from a stream
		/// </summary>
		/// <param name="stream">Stream containing serialized items</param>
		/// <returns>Collection of items (or an empty collection)</returns>
		protected virtual IEnumerable<TextPrompt> Deserialize(Stream stream)
		{
			if (stream.Length == 0)
				return new List<TextPrompt>();

			var serializer = new DataContractJsonSerializer(typeof(List<TextPrompt>));
			return (IEnumerable<TextPrompt>)serializer.ReadObject(stream);
		}

		private string GetFullPath(CultureInfo culture)
		{
			return HostingEnvironment.MapPath(string.Format(@"~/App_Data/TypeLocalization.{0}.dat", culture.Name));
		}


		/// <summary>
		/// Get a localized string for a model property
		/// </summary>
		/// <param name="model">Model being localized</param>
		/// <param name="propertyName">Property to get string for</param>
		/// <returns>Translated string if found; otherwise null.</returns>
		public string GetModelString(Type model, string propertyName)
		{
			return Translate(model, propertyName);
		}

		/// <summary>
		/// Get a localized metadata for a model property
		/// </summary>
		/// <param name="model">Model being localized</param>
		/// <param name="propertyName">Property to get string for</param>
		/// <param name="metadataName">Valid names are: Watermark, Description, NullDisplayText, ShortDisplayText.</param>
		/// <returns>Translated string if found; otherwise null.</returns>
		/// <remarks>
		/// Look at <see cref="ModelMetadata"/> to know more about the meta data
		/// </remarks>
		public string GetModelString(Type model, string propertyName, string metadataName)
		{
			return Translate(model, propertyName + "_" + metadataName);
		}

		/// <summary>
		/// Get a translated string for a validation attribute
		/// </summary>
		/// <param name="attributeType">Type of attribute</param>
		/// <returns>Translated validtion message if found; otherwise null.</returns>
		/// <remarks>
		/// Used to get localized error messages for the DataAnnotation attributes. The returned string 
		/// should have the same format as the built in messages, such as "{0} is required.".
		/// </remarks>
		public string GetValidationString(Type attributeType)
		{
			return Translate(attributeType, null);
		}

		/// <summary>
		/// Gets a enum string
		/// </summary>
		/// <param name="enumType">Type of enum</param>
		/// <param name="name">Name of the value to translation for</param>
		/// <returns>Translated name if found; otherwise null.</returns>
		public string GetEnumString(Type enumType, string name)
		{
			return Translate(enumType, name);
		}

		private string Translate(Type type, string name)
		{
			string textToDisplay = null;
			if (type.Namespace.StartsWith("Griffin.MvcContrib.Localization"))
				return name;


			var lang = GetLanguage(CultureInfo.CurrentUICulture);
			if (lang == null)
			{
				var prompt = new TextPrompt
								{
									LocaleId = CultureInfo.CurrentUICulture.LCID,
									Subject = type,
									TextName = name,
									TextKey = TextPrompt.CreateKey(type, name)
								};
				SaveLanguage(CultureInfo.CurrentUICulture, new[] { prompt });
				if (CultureInfo.CurrentUICulture == DefaultCulture)
					textToDisplay = name;
			}
			else
			{
				var prompt = lang.GetPrompt(type, name);
				if (prompt == null)
				{
					prompt = new TextPrompt
								{
									LocaleId = CultureInfo.CurrentUICulture.LCID,
									Subject = type,
									TextName = name,
									TextKey = TextPrompt.CreateKey(type, name)
								};
					lang.Add(prompt);
					SaveLanguage(CultureInfo.CurrentUICulture, lang);
				}
				else
					textToDisplay = prompt.TranslatedText;
			}

			if (textToDisplay != null)
				return textToDisplay;

			if (name.EndsWith("NullDisplayText"))
				return "";

			return string.Format("{0}: [{1}.{2}]", CultureInfo.CurrentUICulture.Name, type.Name, name);

		}

		public IEnumerable<TextPrompt> GetPrompts(CultureInfo cultureInfo)
		{
			return GetLanguage(cultureInfo);
		}

		public TextPrompt GetPrompt(CultureInfo culture, string key)
		{
			return GetLanguage(culture).Get(key);
		}

		public void Update(CultureInfo culture, string textKey, string translatedText)
		{
			var lang = GetLanguage(culture);
			var prompt = lang.Get(textKey);
			prompt.TranslatedText = translatedText;
			SaveLanguage(culture, lang);
		}

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

	}
}
