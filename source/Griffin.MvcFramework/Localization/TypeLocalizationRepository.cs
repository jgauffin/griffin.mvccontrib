using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Griffin.MvcContrib.Localization.Types;
using Raven.Client;

namespace Griffin.MvcContrib.RavenDb.Localization
{
	/// <summary>
	/// Used to translate different types (and their properties) 
	/// </summary>
	public class TypeLocalizationRepository : ILocalizedStringProvider, ILocalizedTypesRepository
	{
		private readonly IDocumentSession _documentSession;

		public CultureInfo DefaultCulture { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeLocalizationRepository"/> class.
		/// </summary>
		/// <param name="documentSession">The document session used to work with the database.</param>
		public TypeLocalizationRepository(IDocumentSession documentSession)
		{
			_documentSession = documentSession;
			DefaultCulture = new CultureInfo(1033);
			
		}

		#region Implementation of ILocalizedStringProvider

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
			return Translate(attributeType, "");
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

		#endregion

		#region Implementation of ILocalizedTypesRepository

		/// <summary>
		/// Get all prompts
		/// </summary>
		/// <param name="cultureInfo">Culture to get prompts for</param>
		/// <returns>Collection of translations</returns>
		public IEnumerable<TextPrompt> GetPrompts(CultureInfo cultureInfo)
		{
			return (from p in _documentSession.Query<TypePrompt>()
					where p.LocaleId == cultureInfo.LCID
					select CreateTextPrompt(p)).ToList();
		}

		private static TextPrompt CreateTextPrompt(TypePrompt p)
		{
			return new TextPrompt
			       	{
			       		LocaleId = p.LocaleId,
			       		Subject = Type.GetType(string.Format("{0}, {1}", p.FullTypeName,p.AssemblyName)),
			       		TextName = p.TextName,
			       		TextKey = p.TextKey,
			       		TranslatedText = p.Text,
			       		UpdatedAt = p.UpdatedAt,
			       		UpdatedBy = p.UpdatedBy
			       	};
		}

		/// <summary>
		/// Get a specific prompt
		/// </summary>
		/// <param name="culture">Culture to get prompt for</param>
		/// <param name="key">Unique key, in the specified language only, for the prompt to get)</param>
		/// <returns>Prompt if found; otherwise <c>null</c>.</returns>
		public TextPrompt GetPrompt(CultureInfo culture, string key)
		{
			return (from p in _documentSession.Query<TypePrompt>()
			        where p.LocaleId == culture.LCID && p.TextKey == key
			        select CreateTextPrompt(p)).FirstOrDefault();
		}

		/// <summary>
		/// Update translation
		/// </summary>
		/// <param name="culture">Culture that the prompt is for</param>
		/// <param name="key">Unique key, in the specified language only, for the prompt to get)</param>
		/// <param name="translatedText">Translated text string</param>
		public void Update(CultureInfo culture, string key, string translatedText)
		{
			var prompt= (from p in _documentSession.Query<TypePrompt>()
			        where p.LocaleId == culture.LCID && p.TextKey == key
			        select p).FirstOrDefault();
			if (prompt == null)
				throw new InvalidOperationException("Prompt " + key + " do not exist.");
			prompt.Text = translatedText;
			_documentSession.Store(prompt);
			_documentSession.SaveChanges();
		}

		/// <summary>
		/// Get all languages that got partial or full translations.
		/// </summary>
		/// <returns>Cultures corresponding to the translations</returns>
		public IEnumerable<CultureInfo> GetAvailableLanguages()
		{
			var languages = from p in _documentSession.Query<TypePrompt>()
			                group p by p.LocaleId
			                into g
			                select new CultureInfo(g.Key);
			return languages.ToList();
		}

		#endregion

		private string Translate(Type model, string propertyName)
		{
			var prompt = GetPrompt(CultureInfo.CurrentUICulture, model, propertyName);
			if (prompt != null)
				return prompt.Text;

			prompt = GetPrompt(DefaultCulture, model, propertyName);
			if (prompt != null)
				return prompt.Text;

			return propertyName;
		}

		private TypePrompt GetPrompt(CultureInfo culture, Type model, string propertyName)
		{
			return (from p in _documentSession.Query<TypePrompt>()
						  where p.FullTypeName == model.FullName
						  select p).FirstOrDefault();
		}
	}
}
