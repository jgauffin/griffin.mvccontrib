using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Griffin.MvcContrib.Localization.Views;
using Raven.Client;

namespace Griffin.MvcContrib.RavenDb.Localization
{
	/// <summary>
	/// RavenDB repository for view localizations
	/// </summary>
	public class ViewLocalizationRepository : IViewLocalizationRepository, IDisposable
	{
		private static readonly Dictionary<int, ViewLocalizationDocument> _cache =
			new Dictionary<int, ViewLocalizationDocument>();

		private readonly IDocumentSession _documentSession;

		private readonly ILogger _logger = LogProvider.Current.GetLogger<ViewLocalizationRepository>();
		private readonly LinkedList<ViewLocalizationDocument> _modifiedDocuments = new LinkedList<ViewLocalizationDocument>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewLocalizationRepository"/> class.
		/// </summary>
		/// <param name="documentSession">The document session.</param>
		public ViewLocalizationRepository(IDocumentSession documentSession)
		{
			_documentSession = documentSession;
			if (DefaultCulture == null)
				DefaultCulture = new CultureInfo(1033);
		}

		public static CultureInfo DefaultCulture { get; set; }

		#region Implementation of IViewLocalizationRepository

		/// <summary>
		/// Get all prompts that have been created for an language
		/// </summary>
		/// <param name="culture">Culture to get translation for</param>
		/// <returns>A collection of prompts</returns>
		public IEnumerable<TextPrompt> GetAllPrompts(CultureInfo culture)
		{
			return GetOrCreateLanguage(culture, culture).Prompts.Select(CreatePrompt);
		}

		/// <summary>
		/// Get all languages that have translations
		/// </summary>
		/// <returns>Collection of languages</returns>
		public IEnumerable<CultureInfo> GetAvailableLanguages()
		{
			var languages = from p in _documentSession.Query<ViewLocalizationDocument>()
			                select p.Id;
			return languages.ToList().Select(p => new CultureInfo(p));
		}

		/// <summary>
		/// Get all prompts that have not been translated
		/// </summary>
		/// <param name="culture">Culture to get translation for</param>
		/// <param name="defaultCulture">Default language</param>
		/// <returns>A collection of prompts</returns>
		/// <remarks>
		/// Default language will typically have more translated prompts than any other language
		/// and is therefore used to detect missing prompts.
		/// </remarks>
		public IEnumerable<TextPrompt> GetNotLocalizedPrompts(CultureInfo culture, CultureInfo defaultCulture)
		{
			var sourceLanguage = GetOrCreateLanguage(defaultCulture, defaultCulture);
			var ourLanguage = GetLanguage(culture) ?? new ViewLocalizationDocument {Id = culture.Name};
			return sourceLanguage.Prompts.Except(ourLanguage.Prompts.Where(p => p.Text != "")).Select(CreatePrompt).ToList();
		}

		/// <summary>
		/// Create a new language
		/// </summary>
		/// <param name="culture">Language to create</param>
		/// <param name="defaultCulture">The default culture.</param>
		/// <remarks>
		/// Will add empty entries for all known entries. Entries are added automatically to the default language when views
		/// are visited. This is NOT done for any other language.
		/// </remarks>
		public void CreateForLanguage(CultureInfo culture, CultureInfo defaultCulture)
		{
			GetOrCreateLanguage(culture, defaultCulture);
		}

		/// <summary>
		/// Get a text using it's name.
		/// </summary>
		/// <param name="culture">Culture to get prompt for</param>
		/// <param name="id">Id of the prompt</param>
		/// <returns>Prompt if found; otherwise null.</returns>
		public TextPrompt GetPrompt(CultureInfo culture, string id)
		{
			var language = GetLanguage(culture);
			if (language == null)
				return null;

			return language.Prompts.Where(p => p.TextKey == id).Select(CreatePrompt).FirstOrDefault();
		}

		/// <summary>
		/// Save/Update a text prompt
		/// </summary>
		/// <param name="prompt">Prompt to update</param>
		public void Save(TextPrompt prompt)
		{
			var ourCulture = new CultureInfo(prompt.LocaleId);
			var language = GetOrCreateLanguage(ourCulture, ourCulture);
			var dbPrompt = language.Prompts.Where(p => p.TextKey == prompt.TextKey).FirstOrDefault();
			if (dbPrompt != null)
				dbPrompt.Text = prompt.TranslatedText;
			else
			{
				dbPrompt = new ViewPrompt(prompt);
				language.Prompts.Add(dbPrompt);
			}

			// Can't figure out a better way.
			if (prompt.ActionName == null)
				prompt.ActionName = "Index";

			_logger.Debug("Saving prompt " + prompt.ControllerName + "." + prompt.ActionName + "." + prompt.TextName);
			_documentSession.Store(language);
			_documentSession.SaveChanges();
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
			if (source.TextKey == null)
				throw new InvalidOperationException("TextKey must be specified for all prompts.");

			var language = GetOrCreateLanguage(culture, culture);
			var dbPrompt = language.Prompts.Where(p => p.TextKey == source.TextKey).FirstOrDefault();
			if (dbPrompt == null)
			{
				_logger.Debug("Created prompt " + source.ControllerName + "." + source.ActionName + "." + source.TextName);
				dbPrompt = new ViewPrompt(source)
				           	{
				           		Text = translatedText,
				           		LocaleId = culture.LCID,
				           	};
				language.Prompts.Add(dbPrompt);
			}
			else
			{
				dbPrompt.Text = translatedText;
			}
			lock (_modifiedDocuments)
			{
				if (!_modifiedDocuments.Contains(language))
					_modifiedDocuments.AddLast(language);
			}
		}

		private ViewLocalizationDocument GetOrCreateLanguage(CultureInfo culture, CultureInfo defaultCulture)
		{
			var document = GetLanguage(culture);
			if (document == null)
			{
				_logger.Debug("Failed to find language " + culture.Name + ", creating it using " + defaultCulture.Name +
				              " as a template.");
				var defaultLang = culture.LCID == defaultCulture.LCID
				                  	? new ViewLocalizationDocument {Id = culture.Name, Prompts = new List<ViewPrompt>()}
				                  	: GetOrCreateLanguage(defaultCulture, defaultCulture);

				document = defaultLang.Clone(culture);
				_documentSession.Store(document);
				_documentSession.SaveChanges();
				lock (_cache)
					_cache[culture.LCID] = document;
			}

			return document;
		}

		private ViewLocalizationDocument GetLanguage(CultureInfo culture)
		{
			ViewLocalizationDocument document;
			lock (_cache)
			{
				if (_cache.TryGetValue(culture.LCID, out document))
					return document;
			}

			document = (from p in _documentSession.Query<ViewLocalizationDocument>()
			            where p.Id == culture.Name
			            select p).FirstOrDefault();
			lock (_cache)
			{
				_cache[culture.LCID] = document;
			}

			return document;
		}

		private TextPrompt CreatePrompt(ViewPrompt prompt)
		{
			return new TextPrompt
			       	{
			       		ActionName = prompt.ActionName,
			       		ControllerName = prompt.ControllerName,
			       		LocaleId = prompt.LocaleId,
			       		TextKey = prompt.TextKey,
			       		TextName = prompt.TextName,
			       		TranslatedText = prompt.Text
			       	};
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			lock (_modifiedDocuments)
			{
				if (_modifiedDocuments.Count > 0)
					_logger.Debug("Writing cache");

				foreach (var document in _modifiedDocuments)
				{
					_documentSession.Store(document);
				}
				_modifiedDocuments.Clear();
				_documentSession.SaveChanges();
			}
		}

		#endregion
	}
}