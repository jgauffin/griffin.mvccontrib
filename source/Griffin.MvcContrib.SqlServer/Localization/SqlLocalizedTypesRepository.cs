using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Providers.Membership.SqlRepository;

namespace Griffin.MvcContrib.SqlServer.Localization
{
    /// <summary>
    /// Used to localize types
    /// </summary>
    public class SqlLocalizedTypesRepository : ILocalizedTypesRepository
    {
        private readonly ILocalizationDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalizedTypesRepository"/> class.
        /// </summary>
        /// <param name="db">Database connection.</param>
        public SqlLocalizedTypesRepository(ILocalizationDbContext db)
        {
            if (db == null) throw new ArgumentNullException("db");
            _db = db;
        }

        #region ILocalizedTypesRepository Members

        /// <summary>
        /// Get all prompts
        /// </summary>
        /// <param name="cultureInfo">Culture to get prompts for</param>
        /// <param name="defaultCulture">Culture used as template to be able to include all non-translated prompts</param>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// Collection of translations
        /// </returns>
        public IEnumerable<TextPrompt> GetPrompts(CultureInfo cultureInfo, CultureInfo defaultCulture, SearchFilter filter)
        {
            var sql = "SELECT * FROM LocalizedTypes WHERE LocaleId = @LocaleId";

            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameter("LocaleId", cultureInfo.LCID);

                using (var reader = cmd.ExecuteReader())
                {
                    var items = new List<TextPrompt>();
                    while (reader.Read())
                    {
                        items.Add(MapEntity(reader));
                    }
                    return items;
                }
            }
        }

        /// <summary>
        /// Create translation for a new language
        /// </summary>
        /// <param name="culture">Language to create</param>
        /// <param name="templateCulture">Language to use as a template</param>
        public void CreateLanguage(CultureInfo culture, CultureInfo templateCulture)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (templateCulture == null) throw new ArgumentNullException("templateCulture");

            var sql =
                @"INSERT INTO LocalizedTypes (LocaleId, TypeName, TextName, [Key], Value, UpdatedAt, UpdatedBy)
                      SELECT {3}, TypeName, TextName, [Key], Value, '{0}', '{1}'
                    FROM LocalizedTypes WHERE LocaleId={2}";
            sql = string.Format(sql, DateTime.Now, Thread.CurrentPrincipal.Identity.Name, templateCulture.LCID, culture.LCID);
            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
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
            var sql = "SELECT * FROM LocalizedTypes WHERE LocaleId = @LocaleId AND [Key] = @TextKey";
            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameter("LocaleId", culture.LCID);
                cmd.AddParameter("TextKey", key.ToString());

                using (var reader = cmd.ExecuteReader())
                {
                    return !reader.Read() ? null : MapEntity(reader);
                }
            }
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
            if (culture == null) throw new ArgumentNullException("culture");
            if (type == null) throw new ArgumentNullException("type");
            if (name == null) throw new ArgumentNullException("name");
            if (translatedText == null) throw new ArgumentNullException("translatedText");

            var key = new TypePromptKey(type, name);
            if (!Exists(culture, key))
                Create(culture, type, name, translatedText);
            else
                Update(culture, key , translatedText);
        }

        /// <summary>
        /// Get all languages that got partial or full translations.
        /// </summary>
        /// <returns>Cultures corresponding to the translations</returns>
        public IEnumerable<CultureInfo> GetAvailableLanguages()
        {
            var sql = "SELECT distinct LocaleId FROM LocalizedTypes";
            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    var items = new List<CultureInfo>();
                    while (reader.Read())
                    {
                        items.Add(new CultureInfo((int) reader[0]));
                    }
                    return items;
                }
            }
        }

        #endregion

        private void Create(CultureInfo culture, Type type, string name, string translatedText)
        {
            var sql =
                @"INSERT INTO LocalizedTypes (LocaleId, [Key], TypeName, TextName, Value, UpdatedAt, UpdatedBy)
                      VALUES (@lcid, @TextKey, @TypeName, @TextName, @value, @updat, @updby)";

            var key = new TypePromptKey(type, name);
            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.AddParameter("lcid", culture.LCID);
                cmd.AddParameter("TextKey", key.ToString());
                cmd.AddParameter("TypeName", type.AssemblyQualifiedName);
                cmd.AddParameter("TextName", name);
                cmd.AddParameter("value", translatedText);
                cmd.AddParameter("updat", DateTime.Now);
                cmd.AddParameter("updby", Thread.CurrentPrincipal.Identity.Name);
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        private bool Exists(CultureInfo culture, TypePromptKey key)
        {
            var sql = @"SELECT count(Id) FROM LocalizedTypes WHERE LocaleId = @lcid AND [Key] = @key";

            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.AddParameter("lcid", culture.LCID);
                cmd.AddParameter("key", key.ToString());
                cmd.CommandText = sql;
                return !cmd.ExecuteScalar().Equals(0);
            }
        }

        private TextPrompt MapEntity(IDataRecord record)
        {
            return new TextPrompt
                    {
                        LocaleId = (int) record["LocaleId"],
                        SubjectTypeName = record["TypeName"].ToString(),
                        Key = new TypePromptKey(record["Key"].ToString()),
                        TextName = record["TextName"].ToString(),
                        TranslatedText = record["Value"].ToString(),
                        UpdatedAt = (DateTime) record["UpdatedAt"],
                        UpdatedBy = record["UpdatedBy"].ToString()
                    };
        }

        /// <summary>
        /// Update translation
        /// </summary>
        /// <param name="culture">Culture that the prompt is for</param>
        /// <param name="key">Unique key, in the specified language only, for the prompt to get)</param>
        /// <param name="translatedText">Translated text string</param>
        public void Update(CultureInfo culture, TypePromptKey key, string translatedText)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (key == null) throw new ArgumentNullException("key");
            if (translatedText == null) throw new ArgumentNullException("translatedText");

            var sql = "UPDATE LocalizedTypes SET Value=@value WHERE LocaleId = @lcid AND [Key] = @key";

            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameter("value", translatedText);
                cmd.AddParameter("lcid", culture.LCID);
                cmd.AddParameter("key", key.ToString());
                cmd.ExecuteNonQuery();
            }
        }
    }
}