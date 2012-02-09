using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
    public class ViewLocalizationFileRepository : IViewLocalizationRepository
    {
        private static readonly object WriteLock = new object();

        private readonly Dictionary<CultureInfo, TextPromptCollection> _languages =
            new Dictionary<CultureInfo, TextPromptCollection>();

        #region IViewLocalizationRepository Members

        /// <summary>
        /// Get all prompts that have been created for an language
        /// </summary>
        /// <param name="culture">Culture to get translation for</param>
        /// <param name="templateCulture">Culture to find not translated prompts in (or same culture to disable)</param>
        /// <param name="filter">Used to limit the search result</param>
        /// <returns>
        /// A collection of prompts
        /// </returns>
        public IEnumerable<TextPrompt> GetAllPrompts(CultureInfo culture, CultureInfo templateCulture, SearchFilter filter)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (filter == null) throw new ArgumentNullException("filter");

            var ourLanguage = GetLanguage(culture);
            if (templateCulture == null || culture == templateCulture)
                return ourLanguage;

            var defaultLanguage = GetLanguage(templateCulture);
            var missing = defaultLanguage.Except(ourLanguage, new PromptEqualityComparer())
                .Select(p => new TextPrompt(culture.LCID, p));
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
        /// Get all languages that have translations
        /// </summary>
        /// <returns>Collection of languages</returns>
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


        /// <summary>
        /// Get a text using it's name.
        /// </summary>
        /// <param name="culture">Culture to get prompt for</param>
        /// <param name="key"> </param>
        /// <returns>
        /// Prompt if found; otherwise null.
        /// </returns>
        public TextPrompt GetPrompt(CultureInfo culture, ViewPromptKey key)
        {
            var prompts = GetLanguage(CultureInfo.CurrentUICulture);
            return prompts.Get(key);
        }

        /// <summary>
        /// Save/Update a text prompt
        /// </summary>
        /// <param name="culture">Language to save prompt in</param>
        /// <param name="viewPath">Path to view. You can use <see cref="ViewPromptKey.GetViewPath"/></param>
        /// <param name="textName">Text to translate</param>
        /// <param name="translatedText">Translated text</param>
        public void Save(CultureInfo culture, string viewPath, string textName, string translatedText)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (viewPath == null) throw new ArgumentNullException("viewPath");
            if (textName == null) throw new ArgumentNullException("textName");
            if (translatedText == null) throw new ArgumentNullException("translatedText");

            var prompts = GetLanguage(CultureInfo.CurrentUICulture);
            var key = new ViewPromptKey(viewPath, textName);
            var thePrompt = prompts.Get(key);
            if (thePrompt == null)
            {
                prompts.Add(new TextPrompt
                                {
                                    Key = key,
                                    LocaleId = culture.LCID,
                                    TextName = textName,
                                    TranslatedText = translatedText,
                                    ViewPath = viewPath
                                });
            }
            else
                thePrompt.TranslatedText = translatedText;

            SaveLanguage(prompts.Culture, prompts);
        }

        /// <summary>
        /// Existses the specified culture info.
        /// </summary>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns></returns>
        public bool Exists(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) throw new ArgumentNullException("cultureInfo");
            return GetLanguage(cultureInfo) != null;
        }

        /// <summary>
        /// Create a new prompt in the specified language
        /// </summary>
        /// <param name="culture">Language that the translation is for</param>
        /// <param name="viewPath">Path to view. You can use <see cref="ViewPromptKey.GetViewPath"/></param>
        /// <param name="textName">Text to translate</param>
        /// <param name="translatedText">Translated text</param>
        public void CreatePrompt(CultureInfo culture, string viewPath, string textName, string translatedText)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (viewPath == null) throw new ArgumentNullException("viewPath");
            if (textName == null) throw new ArgumentNullException("textName");
            if (translatedText == null) throw new ArgumentNullException("translatedText");

            var key = new ViewPromptKey(viewPath, textName);
            var prompt = new TextPrompt
                            {
                                Key = key,
                                TranslatedText = translatedText,
                                LocaleId = culture.LCID,
                                TextName = textName,
                                ViewPath = viewPath
                            };
            var language = GetLanguage(culture);
            if (language == null)
            {
                var prompts = GetAllPrompts(culture, DefaultCulture.Value, new SearchFilter());
                var collection = new TextPromptCollection(culture);
                collection.AddRange(prompts);

                // dont forget to translate
                var dbPrompt = collection.Get(key);
                if (dbPrompt == null)
                    collection.Add(prompt);
                else
                    dbPrompt.TranslatedText = translatedText;

                language = _languages[culture];
            }
            else
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
            if (stream.Length == 0)
                return new List<TextPrompt>();
            
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