using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Griffin.MvcContrib.Localization.Types;
using Raven.Client;

namespace Griffin.MvcContrib.RavenDb.Localization
{
	/// <summary>
	/// Used to translate different types (and their properties) 
	/// </summary>
	/// <remarks>
	/// <para>You might want to specify <see cref="DefaultCulture"/>, en-us is used per default.</para>
	/// <para>
	/// Class is not thread safe and are expected to have a short lifetime (per scope)
	/// </para></remarks>
	public class TypeLocalizationRepository : ILocalizedStringProvider, ILocalizedTypesRepository, IDisposable
	{
		private static readonly Dictionary<int, TypeLocalizationDocument> _cache =
			new Dictionary<int, TypeLocalizationDocument>();

		private readonly IDocumentSession _documentSession;

		private readonly ILogger _logger = LogProvider.Current.GetLogger<TypeLocalizationRepository>();
		private readonly LinkedList<TypeLocalizationDocument> _modifiedDocuments = new LinkedList<TypeLocalizationDocument>();

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeLocalizationRepository"/> class.
		/// </summary>
		/// <param name="documentSession">The document session used to work with the database.</param>
		public TypeLocalizationRepository(IDocumentSession documentSession)
		{
			_documentSession = documentSession;
			DefaultCulture = new CultureInfo(1033);
			CheckValidationPrompts();
		}

		private void CheckValidationPrompts()
		{
			var language = GetOrCreateLanguage(DefaultCulture);
			var prompt = language.Get(typeof (RequiredAttribute), null);
			if (prompt != null)
				return;

			var baseAttribte = typeof (ValidationAttribute);
			var attributes =
				typeof (RequiredAttribute).Assembly.GetTypes().Where(p => baseAttribte.IsAssignableFrom(p) && !p.IsAbstract).ToList();
			foreach (var type in attributes)
			{
				var key = TextPrompt.CreateKey(type, "");
				var typePrompt = new TypePrompt(key, type, null, DefaultCulture);
				language.AddPrompt(typePrompt);
			}

			_documentSession.Store(language);
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
			return Translate(model, propertyName, null);
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
			return Translate(model, propertyName, metadataName);
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
			return Translate(attributeType, null, null);
		}

		/// <summary>
		/// Gets a enum string
		/// </summary>
		/// <param name="enumType">Type of enum</param>
		/// <param name="name">Name of the value to translation for</param>
		/// <returns>Translated name if found; otherwise null.</returns>
		public string GetEnumString(Type enumType, string name)
		{
			return Translate(enumType, name, null);
		}

		private TypeLocalizationDocument GetOrCreateLanguage(CultureInfo culture)
		{
			TypeLocalizationDocument document;
			lock (_cache)
			{
				if (_cache.TryGetValue(culture.LCID, out document))
					return document;
			}

			document = (from p in _documentSession.Query<TypeLocalizationDocument>()
			            where p.Id == culture.Name
			            select p).FirstOrDefault();
			if (document == null)
			{
				_logger.Debug("Failed to find document for " + culture.Name + ", creating it.");
				var defaultLang = DefaultCulture.LCID == culture.LCID
				                  	? new TypeLocalizationDocument {Id = culture.Name, Prompts = new List<TypePrompt>()}
				                  	: GetOrCreateLanguage(DefaultCulture);

				document = defaultLang.Clone(culture);
				_documentSession.Store(document);
				_documentSession.SaveChanges();
			}

			lock (_cache)
				_cache[culture.LCID] = document;

			return document;
		}

		#endregion

		#region Implementation of ILocalizedTypesRepository

		/// <summary>
		/// Get all prompts
		/// </summary>
		/// <param name="cultureInfo">Culture to get prompts for</param>
		/// <returns>Collection of translations</returns>
		public IEnumerable<TextPrompt> GetPrompts(CultureInfo cultureInfo, CultureInfo defaultCulture)
		{
			var ourDocument = GetOrCreateLanguage(cultureInfo);
			if (defaultCulture == null || defaultCulture == cultureInfo)
				return ourDocument.Prompts.Select(CreateTextPrompt);

			// get all prompts including not localized ones
			var defaultDocument = GetOrCreateLanguage(defaultCulture);
			var defaultPrompts =
				defaultDocument.Prompts.Except(ourDocument.Prompts, new PromptEqualityComparer()).Select(p => new TypePrompt(p)
				                                                                                              	{
				                                                                                              		LocaleId =
				                                                                                              			cultureInfo.LCID,
				                                                                                              		Text = "",
				                                                                                              		UpdatedAt =
				                                                                                              			DateTime.MinValue,
				                                                                                              		UpdatedBy = ""
				                                                                                              	});
			return ourDocument.Prompts.Union(defaultPrompts).Select(CreateTextPrompt);
		}


		/// <summary>
		/// Get a specific prompt
		/// </summary>
		/// <param name="culture">Culture to get prompt for</param>
		/// <param name="key">Unique key, in the specified language only, for the prompt to get)</param>
		/// <returns>Prompt if found; otherwise <c>null</c>.</returns>
		public TextPrompt GetPrompt(CultureInfo culture, string key)
		{
			var language = GetOrCreateLanguage(culture);
			return (from p in language.Prompts
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
			var language = GetOrCreateLanguage(culture);
			var prompt = (from p in language.Prompts
			              where p.LocaleId == culture.LCID && p.TextKey == key
			              select p).FirstOrDefault();
			if (prompt == null)
				throw new InvalidOperationException("Prompt " + key + " do not exist.");

			prompt.Text = translatedText;
			_logger.Debug("Updating text for " + prompt.TypeName + "." + prompt.TextName + " to " + translatedText);
			_documentSession.Store(language);
			_documentSession.SaveChanges();
		}

		/// <summary>
		/// Get all languages that got partial or full translations.
		/// </summary>
		/// <returns>Cultures corresponding to the translations</returns>
		public IEnumerable<CultureInfo> GetAvailableLanguages()
		{
			var languages = from p in _documentSession.Query<TypeLocalizationDocument>()
			                select p.Id;
			return languages.ToList().Select(p => new CultureInfo(p));
		}

		private static TextPrompt CreateTextPrompt(TypePrompt p)
		{
			var type = Type.GetType(p.AssemblyQualifiedName);
			//var type = Type.GetType(string.Format("{0}, {1}", p.FullTypeName, p.AssemblyName), true);
			return new TextPrompt
			       	{
			       		LocaleId = p.LocaleId,
			       		Subject = type,
			       		TextName = p.TextName,
			       		TextKey = p.TextKey,
			       		TranslatedText = p.Text,
			       		UpdatedAt = p.UpdatedAt,
			       		UpdatedBy = p.UpdatedBy
						
			       	};
		}

		private class PromptEqualityComparer : IEqualityComparer<TypePrompt>
		{
			#region IEqualityComparer<TypePrompt> Members

			/// <summary>
			/// Determines whether the specified objects are equal.
			/// </summary>
			/// <returns>
			/// true if the specified objects are equal; otherwise, false.
			/// </returns>
			/// <param name="x">The first object of type <paramref name="T"/> to compare.</param><param name="y">The second object of type <paramref name="T"/> to compare.</param>
			public bool Equals(TypePrompt x, TypePrompt y)
			{
				return x.TextKey == y.TextKey;
			}

			/// <summary>
			/// Returns a hash code for the specified object.
			/// </summary>
			/// <returns>
			/// A hash code for the specified object.
			/// </returns>
			/// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
			public int GetHashCode(TypePrompt obj)
			{
				return obj.TextKey.GetHashCode();
			}

			#endregion
		}

		#endregion

		public static CultureInfo DefaultCulture { get; set; }

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

		private string Translate(Type model, string propertyName, string metaName)
		{
			var textName = string.IsNullOrEmpty(metaName)
			          	? propertyName
			          	: string.Format("{0}_{1}", propertyName, metaName);
			var language = GetOrCreateLanguage(CultureInfo.CurrentUICulture);
			var prompt = language.Get(model, propertyName);
			if (prompt == null)
			{
				_logger.Debug("Prompt for " + model.Name + "." + propertyName + "." + metaName + " did not exist.");
				var key = TextPrompt.CreateKey(model, textName);
				language.AddPrompt(new TypePrompt(key, model, textName, CultureInfo.CurrentUICulture));
				language = GetOrCreateLanguage(DefaultCulture);
				prompt = language.Get(model, propertyName);
				lock (_modifiedDocuments)
				{
					if (!_modifiedDocuments.Contains(language))
						_modifiedDocuments.AddLast(language);
				}
			}

			return prompt == null ? null : prompt.Text;
		}
	}
}