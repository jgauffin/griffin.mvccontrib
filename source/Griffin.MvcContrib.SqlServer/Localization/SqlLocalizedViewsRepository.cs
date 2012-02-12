using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Views;
using Griffin.MvcContrib.Providers.Membership.SqlRepository;

namespace Griffin.MvcContrib.SqlServer.Localization
{
    /// <summary>
    /// Repository using SQL to handle views.
    /// </summary>
    public class SqlLocalizedViewsRepository : IViewLocalizationRepository
    {
        private readonly ILocalizationDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalizedViewsRepository"/> class.
        /// </summary>
        /// <param name="db">Connection to the database. Typically created per request.</param>
        public SqlLocalizedViewsRepository(ILocalizationDbContext db)
        {
            if (db == null) throw new ArgumentNullException("db");
            _db = db;
        }

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
        public IEnumerable<ViewPrompt> GetAllPrompts(CultureInfo culture, CultureInfo templateCulture,
                                                     SearchFilter filter)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            var sql =
                "SELECT LocaleId, [Key], ViewPath, TextName, Value, UpdatedAt, UpdatedBy FROM LocalizedViews WHERE LocaleId = @LocaleId";
            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameter("LocaleId", culture.LCID);
                return MapCollection(cmd);
            }
        }

        /// <summary>
        /// Get all languages that have translations
        /// </summary>
        /// <returns>Collection of languages</returns>
        public IEnumerable<CultureInfo> GetAvailableLanguages()
        {
            var sql = "SELECT distinct LocaleId FROM LocalizedViews";
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

        /// <summary>
        /// Create a new language
        /// </summary>
        /// <param name="culture">Language to create</param>
        /// <param name="defaultCulture">Culture to copy prompts from</param>
        /// <remarks>
        /// Will add empty entries for all known entries. Entries are added automatically to the default language when views
        /// are visited. This is NOT done for any other language.
        /// </remarks>
        public void CreateLanguage(CultureInfo culture, CultureInfo defaultCulture)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (defaultCulture == null) throw new ArgumentNullException("defaultCulture");

            var sql =
                @"INSERT INTO LocalizedViews (LocaleId, ViewPath, TextName, [Key], Value, UpdatedAt, UpdatedBy)
                      SELECT {3}, ViewPath, TextName, [Key], Value, '{0}', '{1}'
                    FROM LocalizedViews WHERE LocaleId={2}";
            sql = string.Format(sql, DateTime.Now, Thread.CurrentPrincipal.Identity.Name, defaultCulture.LCID,
                                culture.LCID);
            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Get a text using it's name.
        /// </summary>
        /// <param name="culture">Culture to get prompt for</param>
        /// <param name="key"> </param>
        /// <returns>Prompt if found; otherwise null.</returns>
        public ViewPrompt GetPrompt(CultureInfo culture, ViewPromptKey key)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (key == null) throw new ArgumentNullException("key");

            var sql = @"SELECT * FROM LocalizedViews WHERE LocaleId = @LocaleId AND [Key] = @key";

            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.AddParameter("LocaleId", culture.LCID);
                cmd.AddParameter("TextKey", key.ToString());
                cmd.CommandText = sql;
                cmd.AddParameter("key", key.ToString());
                using (var reader = cmd.ExecuteReader())
                {
                    return !reader.Read() ? null : MapEntity(reader);
                }
            }
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

            var prompt = new ViewPrompt
                             {
                                 Key = new ViewPromptKey(viewPath, textName),
                                 LocaleId = culture.LCID,
                                 TextName = textName,
                                 TranslatedText = translatedText,
                                 ViewPath = viewPath
                             };

            if (Exists(culture, prompt.Key.ToString()))
                Update(prompt);
            else
                Create(prompt);
        }

        public bool Exists(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) throw new ArgumentNullException("cultureInfo");

            var sql = @"SELECT count(Id) FROM LocalizedViews WHERE LocaleId = @lcid";

            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.AddParameter("lcid", cultureInfo.LCID);
                cmd.CommandText = sql;
                return !cmd.ExecuteScalar().Equals(0);
            }
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

            var prompt = new ViewPrompt
                             {
                                 Key = new ViewPromptKey(viewPath, textName),
                                 LocaleId = culture.LCID,
                                 TextName = textName,
                                 TranslatedText = translatedText,
                                 ViewPath = viewPath
                             };
            Create(prompt);
        }

        /// <summary>
        /// Delete a prompt
        /// </summary>
        /// <param name="cultureInfo">Culture to delete the prompt for</param>
        /// <param name="key">Prompt key</param>
        public void Delete(CultureInfo cultureInfo, ViewPromptKey key)
        {
            if (cultureInfo == null) throw new ArgumentNullException("cultureInfo");
            if (key == null) throw new ArgumentNullException("key");

            var sql =
                @"DELETE FROM LocalizedViews WHERE LocaleId=@lcid AND [Key]=@key";

            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.AddParameter("lcid", cultureInfo.LCID);
                cmd.AddParameter("key", key.ToString());
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

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
        public IEnumerable<ViewPrompt> GetNotLocalizedPrompts(CultureInfo culture, CultureInfo defaultCulture)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (defaultCulture == null) throw new ArgumentNullException("defaultCulture");
            var sql =
                string.Format(
                    @"SELECT src.*
                                        FROM LocalizedViews src 
                                        LEFT JOIN LocalizedViews dst ON (src.TextKey = dst.TextKey) 
                                        WHERE dst.LocaleId = {0} AND src.LocaleId = {1}
                                        AND dst.Value IS NULL",
                    defaultCulture.LCID, culture.LCID);

            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                return MapCollection(cmd);
            }
        }

        private IEnumerable<ViewPrompt> MapCollection(IDbCommand cmd)
        {
            using (var reader = cmd.ExecuteReader())
            {
                var items = new List<ViewPrompt>();
                while (reader.Read())
                {
                    items.Add(MapEntity(reader));
                }
                return items;
            }
        }

        private ViewPrompt MapEntity(IDataRecord record)
        {
            return new ViewPrompt
                       {
                           LocaleId = (int) record["LocaleId"],
                           ViewPath = record["ViewPath"].ToString(),
                           Key = new ViewPromptKey(record["Key"].ToString()),
                           TextName = record["TextName"].ToString(),
                           TranslatedText = record["Value"].ToString(),
                       };
        }

        private void Update(ViewPrompt prompt)
        {
            var sql =
                @"UPDATE LocalizedViews SET Value = @value, UpdatedAt = @updat, UpdatedBy = @updby WHERE LocaleId=@lcid AND [Key]=@key";

            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.AddParameter("lcid", prompt.LocaleId);
                cmd.AddParameter("key", prompt.Key.ToString());
                cmd.AddParameter("value", prompt.TranslatedText);
                cmd.AddParameter("updat", DateTime.Now);
                cmd.AddParameter("updby", Thread.CurrentPrincipal.Identity.Name);
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        private void Create(ViewPrompt prompt)
        {
            var sql =
                @"INSERT INTO LocalizedViews (LocaleId, ViewPath, TextName, [Key], Value, UpdatedAt, UpdatedBy)
                      VALUES (@lcid, @ViewPath, @textName, @key, @value, @updat, @updby)";

            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.AddParameter("lcid", prompt.LocaleId);
                cmd.AddParameter("ViewPath", prompt.ViewPath);
                cmd.AddParameter("textname", prompt.TextName);
                cmd.AddParameter("key", prompt.Key.ToString());
                cmd.AddParameter("value", prompt.TranslatedText);
                cmd.AddParameter("updat", DateTime.Now);
                cmd.AddParameter("updby", Thread.CurrentPrincipal.Identity.Name);
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        private bool Exists(CultureInfo cultureInfo, string textKey)
        {
            var sql = @"SELECT count(Id) FROM LocalizedViews WHERE LocaleId = @lcid AND [Key] = @key";

            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.AddParameter("lcid", cultureInfo.LCID);
                cmd.AddParameter("key", textKey);
                cmd.CommandText = sql;
                return !cmd.ExecuteScalar().Equals(0);
            }
        }
    }
}