using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Web.Hosting;

namespace Griffin.MvcContrib.Localization.Types
{
    /// <summary>
    /// Uses to localize everything for <see cref="ILocalizedStringProvider"/> in files that are placed in the AppData folder.
    /// </summary>
    public class FileLocalizedTypeRepository : ILocalizedTypesRepository
    {
        private static readonly object WriteLock = new object();

        private readonly Dictionary<CultureInfo, TextPromptCollection> _languages =
            new Dictionary<CultureInfo, TextPromptCollection>();


        #region ILocalizedTypesRepository Members

        /// <summary>
        /// Get all prompts
        /// </summary>
        /// <param name="cultureInfo">Culture to get prompts for</param>
        /// <param name="templateCulture">Culture used as template to be able to include all non-translated prompts</param>
        /// <param name="filter">Filter to limit the search result </param>
        /// <returns>
        /// Collection of translations
        /// </returns>
        public IEnumerable<TextPrompt> GetPrompts(CultureInfo cultureInfo, CultureInfo templateCulture, SearchFilter filter)
        {
            var ourLanguage = GetLanguage(cultureInfo);
            if (templateCulture == null || cultureInfo == templateCulture)
                return ourLanguage;

            var defaultLanguage = GetLanguage(templateCulture);
            var missing = defaultLanguage.Except(ourLanguage, new PromptEqualityComparer())
                .Select(p => new TextPrompt(cultureInfo.LCID, p));
            return ourLanguage.Union(missing);
        }

        /// <summary>
        /// Create translation for a new language
        /// </summary>
        /// <param name="culture">Language to create</param>
        /// <param name="templateCulture">Language to use as a template</param>
        public void CreateLanguage(CultureInfo culture, CultureInfo templateCulture)
        {
            var prompts = GetLanguage(templateCulture).Select(p => new TextPrompt(culture.LCID, p));
            var newLanguage = new TextPromptCollection(culture);
            newLanguage.AddRange(prompts);
            lock (_languages)
            {
                _languages[culture] = newLanguage;
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
        public TextPrompt GetPrompt(CultureInfo culture, TypePromptKey key)
        {
            return GetLanguage(culture).Get(key);
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
            var lang = GetLanguage(culture);
            var key = new TypePromptKey(type, name);
            var prompt = lang.Get(key) ?? new TextPrompt
                                            {
                                                Key = key,
                                                LocaleId = culture.LCID,
                                                Subject = type,
                                                TextName = name,
                                                TranslatedText = translatedText,
                                                UpdatedAt = DateTime.Now,
                                                UpdatedBy = Thread.CurrentPrincipal.Identity.Name
                                            };
            prompt.TranslatedText = translatedText;
            SaveLanguage(culture, lang);
        }

        /// <summary>
        /// Uipdates the specified culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="key">The key.</param>
        /// <param name="translatedText">The translated text.</param>
        public void Update(CultureInfo culture, TypePromptKey key, string translatedText)
        {
            var lang = GetLanguage(culture);
            var prompt = lang.Get(key);
            if (prompt == null)
                throw new InvalidOperationException("Failed to find prompt " + key);
            prompt.TranslatedText = translatedText;
            SaveLanguage(culture, lang);
        }

        /// <summary>
        /// Get all languages that got partial or full translations.
        /// </summary>
        /// <returns>
        /// Cultures corresponding to the translations
        /// </returns>
        public IEnumerable<CultureInfo> GetAvailableLanguages()
        {
            var fullPath = GetFullPath(new CultureInfo(1053)).ToLower().Replace("sv-se", "*");

            var directory = Path.GetDirectoryName(fullPath);
            if (directory == null)
                throw new InvalidOperationException("Failed to get path from " + fullPath);
            var filename = Path.GetFileName(fullPath);
            if (filename == null)
                throw new InvalidOperationException("Failed to get filename from " + fullPath);

            var files = Directory.GetFiles(directory, filename);
            var cultures = new List<CultureInfo>();
            foreach (var file in files)
            {
                var withoutDat = Path.GetFileNameWithoutExtension(file);
                if (string.IsNullOrEmpty(withoutDat))
                    continue;
                var langCode = Path.GetExtension(withoutDat);
                if (string.IsNullOrEmpty(langCode))
                    continue;

                var languageCode = langCode.TrimStart('.');
                var culture = new CultureInfo(languageCode);
                cultures.Add(culture);
            }

            return cultures;
        }

        #endregion

        /// <summary>
        /// Serialize items into the specified stream 
        /// </summary>
        /// <param name="stream">Stream to serialize to</param>
        /// <param name="prompts">Prompts to serialize</param>
        protected virtual void Serialize(Stream stream, List<TextPrompt> prompts)
        {
            var serializer = new DataContractJsonSerializer(typeof (List<TextPrompt>));
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

            var serializer = new DataContractJsonSerializer(typeof (List<TextPrompt>));
            return (IEnumerable<TextPrompt>) serializer.ReadObject(stream);
        }

        private string GetFullPath(CultureInfo culture)
        {
            return HostingEnvironment.MapPath(string.Format(@"~/App_Data/TypeLocalization.{0}.dat", culture.Name));
        }


        protected TextPromptCollection GetLanguage(CultureInfo culture)
        {
            TextPromptCollection prompts;
            if (_languages.TryGetValue(culture, out prompts))
                return prompts;

            lock (WriteLock)
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
            lock (WriteLock)
            {
                using (var stream = new FileStream(GetFullPath(cultureInfo), FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    Serialize(stream, prompts.ToList());
                }
            }
        }

        /// <summary>
        /// Gets the not localized prompts.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="defaultCulture">The default culture.</param>
        /// <returns></returns>
        public IEnumerable<TextPrompt> GetNotLocalizedPrompts(CultureInfo culture, CultureInfo defaultCulture)
        {
            var prompts = GetLanguage(culture).Where(p => string.IsNullOrEmpty(p.TranslatedText));
            var defaultPrompts = GetLanguage(defaultCulture);

            return
                defaultPrompts.Except(prompts).Select(
                    source => new TextPrompt(culture.LCID, source)).ToList();
        }

        /// <summary>
        /// Creates for language.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
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

        #region Nested type: PromptEqualityComparer

        private class PromptEqualityComparer : IEqualityComparer<TextPrompt>
        {
            #region IEqualityComparer<TextPrompt> Members

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            /// <param name="x">The first object of type TextPrompt to compare.</param><param name="y">The second object of type TextPrompt to compare.</param>
            public bool Equals(TextPrompt x, TextPrompt y)
            {
                return x.Key == y.Key;
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            /// <returns>
            /// A hash code for the specified object.
            /// </returns>
            /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
            public int GetHashCode(TextPrompt obj)
            {
                return obj.Key.GetHashCode();
            }

            #endregion
        }

        #endregion
    }
}