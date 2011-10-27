using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Griffin.MvcContrib.Localization.Views;
using Raven.Client;

namespace Griffin.MvcContrib.RavenDb.Localization
{
	/// <summary>
	/// RavenDB repository for view localizations
	/// </summary>
	public class ViewLocalizationRepository : IViewLocalizationRepository
	{
		private readonly IDocumentSession _documentSession;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewLocalizationRepository"/> class.
		/// </summary>
		/// <param name="documentSession">The document session.</param>
		public ViewLocalizationRepository(IDocumentSession documentSession)
		{
			_documentSession = documentSession;
		}

		#region Implementation of IViewLocalizationRepository

		/// <summary>
		/// Get all prompts that have been created for an language
		/// </summary>
		/// <param name="culture">Culture to get translation for</param>
		/// <returns>A collection of prompts</returns>
		public IEnumerable<TextPrompt> GetAllPrompts(CultureInfo culture)
		{
			return (from p in _documentSession.Query<ViewPrompt>()
			        where p.LocaleId == culture.LCID
			        select CreatePrompt(p)).ToList();
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

		/// <summary>
		/// Get all languages that have translations
		/// </summary>
		/// <returns>Collection of languages</returns>
		public IEnumerable<CultureInfo> GetAvailableLanguages()
		{
			var languages = from p in _documentSession.Query<TypePrompt>()
							group p by p.LocaleId
								into g
								select new CultureInfo(g.Key);
			return languages.ToList();
		}

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
		public IEnumerable<TextPrompt> GetNotLocalizedPrompts(CultureInfo culture, CultureInfo defaultLanguage)
		{
			var sourceLanguage = GetAllPrompts(defaultLanguage);
			var ourLanguage = GetAllPrompts(culture);
			return sourceLanguage.Except(ourLanguage.Where(p => p.TranslatedText != "")).ToList();

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
			var prompts = from p in _documentSession.Query<ViewPrompt>()
			             where p.LocaleId == culture.LCID
			             select new ViewPrompt
			                    	{
			                    		ActionName = p.ActionName,
			                    		ControllerName = p.ControllerName,
			                    		LocaleId = culture.LCID,
			                    		TextName = p.TextName,
			                    		TextKey = p.TextKey,
			                    		Text = "",
			                    		UpdatedAt = DateTime.Now,
			                    		UpdatedBy = Thread.CurrentPrincipal.Identity.Name
			                    	};

			var i = 0;
			foreach (var prompt in prompts)
			{
				_documentSession.Store(prompt);
				i++;
				if (i % 20 == 0)
					_documentSession.SaveChanges();
			}
			_documentSession.SaveChanges();
		}

		/// <summary>
		/// Get a text using it's name.
		/// </summary>
		/// <param name="culture">Culture to get prompt for</param>
		/// <param name="id">Id of the prompt</param>
		/// <returns>Prompt if found; otherwise null.</returns>
		public TextPrompt GetPrompt(CultureInfo culture, string id)
		{
			return (from p in _documentSession.Query<ViewPrompt>()
			        where p.LocaleId == culture.LCID && p.TextKey == id
			        select CreatePrompt(p)).FirstOrDefault();

		}

		/// <summary>
		/// Save/Update a text prompt
		/// </summary>
		/// <param name="prompt">Prompt to update</param>
		public void Save(TextPrompt prompt)
		{
			var doc = new ViewPrompt(prompt);
			_documentSession.Store(doc);
			_documentSession.SaveChanges();
		}

		public bool Exists(CultureInfo cultureInfo)
		{
			return _documentSession.Query<ViewPrompt>().Any(p => p.LocaleId == cultureInfo.LCID);
		}

		/// <summary>
		/// Create a new prompt in the specified language
		/// </summary>
		/// <param name="culture">Language that the translation is for</param>
		/// <param name="source">Prompt to use as source</param>
		/// <param name="translatedText">Translated text</param>
		public void CreatePrompt(CultureInfo culture, TextPrompt source, string translatedText)
		{
			var prompt = new ViewPrompt(source)
			             	{
			             		Text = translatedText,
			             		LocaleId = culture.LCID,
			             	};
			_documentSession.Store(prompt);
			_documentSession.SaveChanges();
		}

		#endregion
	}
}
