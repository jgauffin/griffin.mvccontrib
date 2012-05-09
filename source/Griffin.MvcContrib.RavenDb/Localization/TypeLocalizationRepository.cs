using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Logging;
using Raven.Client;

namespace Griffin.MvcContrib.RavenDb.Localization
{
    /// <summary>
    /// Used to translate different types (and their properties) 
    /// </summary>
    /// <remarks>
    /// <para>You might want to specify <see cref="DefaultUICulture"/>, en-us is used per default.</para>
    /// <para>
    /// Class is not thread safe and are expected to have a short lifetime (per scope)
    /// </para>
    /// <para>Remember to set <see cref="DefaultUICulture"/></para>
    /// </remarks>
    public class TypeLocalizationRepository : ILocalizedTypesRepository, IDisposable
    {
        private static readonly Dictionary<int, TypeLocalizationDocument> Cache =
            new Dictionary<int, TypeLocalizationDocument>();

        private readonly IDocumentSession _documentSession;

        private readonly ILogger _logger = LogProvider.Current.GetLogger<TypeLocalizationRepository>();

        private readonly LinkedList<TypeLocalizationDocument> _modifiedDocuments =
            new LinkedList<TypeLocalizationDocument>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeLocalizationRepository"/> class.
        /// </summary>
        /// <param name="documentSession">The document session used to work with the database.</param>
        public TypeLocalizationRepository(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
            CheckValidationPrompts();
        }

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

        private void CheckValidationPrompts()
        {
            var language = GetOrCreateLanguage(DefaultUICulture.Value);
            var prompt = language.Get(typeof (RequiredAttribute), "class");
            if (prompt != null)
                return;

            var prompts =
                ValidationAttributesStringProvider.Current.GetPrompts(DefaultUICulture.Value).Select(
                    p => new TypePromptDocument(DefaultUICulture.Value, p)
                             {
                                 Text = p.TranslatedText
                             });

            foreach (var p in prompts)
            {
                language.AddPrompt(p);
            }

            _documentSession.Store(language);
        }

        private TypeLocalizationDocument GetOrCreateLanguage(CultureInfo culture)
        {
            TypeLocalizationDocument document;
            lock (Cache)
            {
                if (Cache.TryGetValue(culture.LCID, out document))
                    return document;
            }

            document = (from p in _documentSession.Query<TypeLocalizationDocument>()
                        where p.Id == culture.Name
                        select p).FirstOrDefault();
            if (document == null)
            {
                _logger.Debug("Failed to find document for " + culture.Name + ", creating it.");
                var defaultLang = DefaultUICulture.Is(culture)
                                      ? new TypeLocalizationDocument
                                            {Id = culture.Name, Prompts = new List<TypePromptDocument>()}
                                      : GetOrCreateLanguage(DefaultUICulture.Value);

                document = defaultLang.Clone(culture);
                _documentSession.Store(document);
                _documentSession.SaveChanges();
            }

            lock (Cache)
                Cache[culture.LCID] = document;

            return document;
        }

        #region Implementation of ILocalizedTypesRepository

        /// <summary>
        /// Get all prompts
        /// </summary>
        /// <param name="cultureInfo">Culture to get prompts for</param>
        /// <param name="defaultCulture">Culture used as template to be able to include all non-translated prompts</param>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// Collection of translations
        /// </returns>
        public IEnumerable<TypePrompt> GetPrompts(CultureInfo cultureInfo, CultureInfo defaultCulture,
                                                  SearchFilter filter)
        {
            var ourDocument = GetOrCreateLanguage(cultureInfo);
            if (defaultCulture == null || defaultCulture == cultureInfo)
                return ourDocument.Prompts.Select(CreateTextPrompt);

            // get all prompts including not localized ones
            var defaultDocument = GetOrCreateLanguage(defaultCulture);
            var defaultPrompts =
                defaultDocument.Prompts.Except(ourDocument.Prompts, new PromptEqualityComparer()).Select(
                    p => new TypePromptDocument(cultureInfo, p)
                             {
                                 UpdatedAt =
                                     DateTime.Now,
                                 UpdatedBy = Thread.CurrentPrincipal.Identity.Name
                             });
            return ourDocument.Prompts.Union(defaultPrompts).Select(CreateTextPrompt);
        }

        /// <summary>
        /// Create translation for a new language
        /// </summary>
        /// <param name="culture">Language to create</param>
        /// <param name="templateCulture">Language to use as a template</param>
        public void CreateLanguage(CultureInfo culture, CultureInfo templateCulture)
        {
            var templateLang = GetOrCreateLanguage(templateCulture);
            var ourLang = new TypeLocalizationDocument
                              {
                                  Id = culture.Name
                              };
            ourLang.Prompts = templateLang.Prompts.Select(p => new TypePromptDocument(culture, p)).ToList();

            lock (_modifiedDocuments)
            {
                _modifiedDocuments.AddLast(ourLang);
            }
        }

        /// <summary>
        /// Get a specific prompt
        /// </summary>
        /// <param name="culture">Culture to get prompt for</param>
        /// <param name="key">Key which is unique in the current language</param>
        /// <returns>
        /// Prompt if found; otherwise <c>null</c>.
        /// </returns>
        public TypePrompt GetPrompt(CultureInfo culture, TypePromptKey key)
        {
            var language = GetOrCreateLanguage(culture);
            return (from p in language.Prompts
                    where p.LocaleId == culture.LCID && p.TextKey == key.ToString()
                    select CreateTextPrompt(p)).FirstOrDefault();
        }

        /// <summary>
        /// Create  or update a prompt
        /// </summary>
        /// <param name="culture">Culture that the prompt is for</param>
        /// <param name="type">Type being localized</param>
        /// <param name="name">Property name and any additonal names (such as metadata name, use underscore as delimiter)</param>
        /// <param name="translatedText">Translated text string</param>
        public void Save(CultureInfo culture, Type type, string name, string translatedText)
        {
            Save(culture, type.FullName, name,translatedText);
        }

        /// <summary>
        /// Create  or update a prompt
        /// </summary>
        /// <param name="culture">Culture that the prompt is for</param>
        /// <param name="fullTypeName">Type.FullName for the type being localized</param>
        /// <param name="name">Property name and any additonal names (such as metadata name, use underscore as delimiter)</param>
        /// <param name="translatedText">Translated text string</param>
        public void Save(CultureInfo culture, string fullTypeName, string name, string translatedText)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (fullTypeName == null) throw new ArgumentNullException("fullTypeName");
            if (name == null) throw new ArgumentNullException("name");
            if (translatedText == null) throw new ArgumentNullException("translatedText");
            if (fullTypeName.IndexOf(".") == -1)
                throw new ArgumentException("You must use Type.FullName", "fullTypeName");

            var pos = fullTypeName.LastIndexOf(".");
            var typeName = fullTypeName.Substring(pos + 1);
            var key = new TypePromptKey(fullTypeName, name);
            var language = GetOrCreateLanguage(culture);
            var prompt = (from p in language.Prompts
                          where p.LocaleId == culture.LCID && p.TextKey == key.ToString()
                          select p).FirstOrDefault() ?? new TypePromptDocument
                          {
                              FullTypeName = fullTypeName,
                              LocaleId = culture.LCID,
                              TextName = name,
                              Text = translatedText,
                              TextKey = key.ToString(),
                              TypeName = typeName,
                              UpdatedAt = DateTime.Now,
                              UpdatedBy = Thread.CurrentPrincipal.Identity.Name
                          };

            prompt.Text = translatedText;
            _logger.Debug("Updating text for " + prompt.TypeName + "." + prompt.TextName + " to " + translatedText);
            _documentSession.Store(language);
            _documentSession.SaveChanges();
        }

        /// <summary>
        /// Updates the specified culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="key">The key.</param>
        /// <param name="translatedText">The translated text.</param>
        public void Update(CultureInfo culture, TypePromptKey key, string translatedText)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (key == null) throw new ArgumentNullException("key");
            if (translatedText == null) throw new ArgumentNullException("translatedText");

            var language = GetOrCreateLanguage(culture);
            var prompt = (from p in language.Prompts
                          where p.LocaleId == culture.LCID && p.TextKey == key.ToString()
                          select p).FirstOrDefault();
            if (prompt == null)
                throw new InvalidOperationException("Prompt " + key + " do not exist.");

            prompt.Text = translatedText;
            _logger.Debug("Updating text for " + prompt.TypeName + "." + prompt.TextName + " to " + translatedText);
            _documentSession.Store(language);
            _documentSession.SaveChanges();
        }

        /// <summary>
        /// Delete a prompt.
        /// </summary>
        /// <param name="culture">Culture to delete the prompt for</param>
        /// <param name="key">Key</param>
        public void Delete(CultureInfo culture, TypePromptKey key)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (key == null) throw new ArgumentNullException("key");

            var language = GetOrCreateLanguage(culture);
            language.DeletePrompt(key);
            _modifiedDocuments.AddLast(language);
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

        private static TypePrompt CreateTextPrompt(TypePromptDocument p)
        {
            //var type = Type.GetType(string.Format("{0}, {1}", p.FullTypeName, p.AssemblyName), true);
            return new TypePrompt
                       {
                           LocaleId = p.LocaleId,
                           TypeFullName = p.FullTypeName,
                           TextName = p.TextName,
                           Key = new TypePromptKey(p.TextKey),
                           TranslatedText = p.Text,
                           UpdatedAt = p.UpdatedAt,
                           UpdatedBy = p.UpdatedBy
                       };
        }

        private class PromptEqualityComparer : IEqualityComparer<TypePromptDocument>
        {
            #region IEqualityComparer<TypePromptDocument> Members

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            /// <param name="x">The first object of type TypePrompt to compare.</param><param name="y">The second object of type TypePrompt to compare.</param>
            public bool Equals(TypePromptDocument x, TypePromptDocument y)
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
            public int GetHashCode(TypePromptDocument obj)
            {
                return obj.TextKey.GetHashCode();
            }

            #endregion
        }

        #endregion
    }
}