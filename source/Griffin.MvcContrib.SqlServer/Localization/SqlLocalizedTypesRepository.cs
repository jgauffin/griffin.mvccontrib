using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Providers.Membership.SqlRepository;

namespace Griffin.MvcContrib.SqlServer.Localization
{
    /// <summary>
    ///   Used to localize types
    /// </summary>
    public class SqlLocalizedTypesRepository : ILocalizedTypesRepository, ITypePromptImporter
    {
        /// <summary>
        /// database context
        /// </summary>
        protected readonly ILocalizationDbContext _db;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlLocalizedTypesRepository" /> class.
        /// </summary>
        /// <param name="db"> Database connection. </param>
        public SqlLocalizedTypesRepository(ILocalizationDbContext db)
        {
            if (db == null) throw new ArgumentNullException("db");
            _db = db;
        }
        

        #region ILocalizedTypesRepository Members

        /// <summary>
        ///   Get all prompts
        /// </summary>
        /// <param name="cultureInfo"> Culture to get prompts for </param>
        /// <param name="defaultCulture"> Culture used as template to be able to include all non-translated prompts </param>
        /// <param name="filter"> The filter. </param>
        /// <returns> Collection of translations </returns>
        public IEnumerable<TypePrompt> GetPrompts(CultureInfo cultureInfo, CultureInfo defaultCulture,
                                                  SearchFilter filter)
        {
            var sql = "SELECT * FROM LocalizedTypes WHERE LocaleId = @LocaleId";

            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameter("LocaleId", cultureInfo.LCID);
                if (!string.IsNullOrEmpty(filter.TextFilter))
                {
                    cmd.CommandText += " AND (TypeName LIKE @TextFilter OR TextName LIKE @TextFilter)";
                    cmd.AddParameter("TextFilter", '%' + filter.TextFilter + "%");
                }
                if (!string.IsNullOrEmpty(filter.Path))
                {
                    cmd.CommandText += " AND TypeName LIKE @PartialName";
                    cmd.AddParameter("PartialName", filter.Path + "%");
                }
                if (filter.OnlyNotTranslated)
                {
                    cmd.CommandText += " AND (Value IS null OR Value LIKE '')";
                }

                using (var reader = cmd.ExecuteReader())
                {
                    var items = new List<TypePrompt>();
                    while (reader.Read())
                    {
                        items.Add(MapEntity(reader));
                    }
                    return items;
                }
            }
        }

        /// <summary>
        ///   Create translation for a new language
        /// </summary>
        /// <param name="culture"> Language to create </param>
        /// <param name="templateCulture"> Language to use as a template </param>
        public void CreateLanguage(CultureInfo culture, CultureInfo templateCulture)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (templateCulture == null) throw new ArgumentNullException("templateCulture");

            var sql =
                @"INSERT INTO LocalizedTypes (LocaleId, TypeName, TextName, [Key], Value, UpdatedAt, UpdatedBy)
                      SELECT {3}, TypeName, TextName, [Key], Value, '{0}', '{1}'
                    FROM LocalizedTypes WHERE LocaleId={2}";
            sql = string.Format(sql, DateTime.Now, Thread.CurrentPrincipal.Identity.Name, templateCulture.LCID,
                                culture.LCID);
            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///   Get a specific prompt
        /// </summary>
        /// <param name="culture"> Culture to get prompt for </param>
        /// <param name="key"> Key which is unique in the current language </param>
        /// <returns> Prompt if found; otherwise <c>null</c> . </returns>
        public TypePrompt GetPrompt(CultureInfo culture, TypePromptKey key)
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
        ///   Create or update a prompt
        /// </summary>
        /// <param name="culture"> Culture that the prompt is for </param>
        /// <param name="type"> Type being localized </param>
        /// <param name="name"> Property name and any additonal names (such as metadata name, use underscore as delimiter) </param>
        /// <param name="translatedText"> Translated text string </param>
        [Obsolete("Use the version with fullTypeName instead")]
        public void Save(CultureInfo culture, Type type, string name, string translatedText)
        {
            Save(culture, type.FullName, name, translatedText);
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

            var key = new TypePromptKey(fullTypeName, name);
            if (!Exists(culture, key))
                Create(culture, fullTypeName, name, translatedText);
            else
                Update(culture, key, translatedText);
        }

        /// <summary>
        ///   Get all languages that got partial or full translations.
        /// </summary>
        /// <returns> Cultures corresponding to the translations </returns>
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
                        items.Add(new CultureInfo((int)reader[0]));
                    }
                    return items;
                }
            }
        }

        /// <summary>
        ///   Update translation
        /// </summary>
        /// <param name="culture"> Culture that the prompt is for </param>
        /// <param name="key"> Unique key, in the specified language only, for the prompt to get) </param>
        /// <param name="translatedText"> Translated text string </param>
        public void Update(CultureInfo culture, TypePromptKey key, string translatedText)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (key == null) throw new ArgumentNullException("key");
            if (translatedText == null) throw new ArgumentNullException("translatedText");

            var sql = "UPDATE LocalizedTypes SET Value=@value, UpdatedAt=@updat, UpdatedBy=@updby WHERE LocaleId = @lcid AND [Key] = @key";

            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameter("value", translatedText);
                cmd.AddParameter("updat", DateTime.Now);
                cmd.AddParameter("updby", Thread.CurrentPrincipal.Identity.Name);
                cmd.AddParameter("lcid", culture.LCID);
                cmd.AddParameter("key", key.ToString());
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///   Delete a prompt.
        /// </summary>
        /// <param name="culture"> Culture to delete prompt in </param>
        /// <param name="key"> Prompt key </param>
        public void Delete(CultureInfo culture, TypePromptKey key)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (key == null) throw new ArgumentNullException("key");

            var sql = "DELETE FROM LocalizedTypes WHERE [Key]=@textKey AND LocaleId = @lcid";
            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameter("lcid", culture.LCID);
                cmd.AddParameter("textKey", key.ToString());
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region ITypePromptImporter Members

        /// <summary>
        ///   Import prompts into the repository.
        /// </summary>
        /// <param name="prompts"> Prompts to import </param>
        /// <remarks>
        /// <para>Batch for inserting/updating several rows.</para>
        /// <para>
        /// This method is not database engine independent (it uses SqlServers MERGE INTO). You must override it to add support
        /// for other databases than SqlServer (for instance using REPLACE INTO in MySQL)</para></remarks>
        public virtual void Import(IEnumerable<TypePrompt> prompts)
        {
            using (var transaction = _db.Connection.BeginTransaction())
            {
                var sql =
                    @"MERGE LocalizedTypes AS target
    USING (SELECT @lcid, @TextKey, @TypeName, @TextName, @value, @updat, @updby) AS source (LocaleId, TextKey, TypeName, TextName, Value, UpdatedAt, UpdatedBy)
    ON (target.LocaleId = source.LocaleId AND target.[Key] = source.TextKey)
    WHEN MATCHED THEN 
        UPDATE SET Value=source.Value, UpdatedAt=source.UpdatedAt, UpdatedBy=source.UpdatedBy
	WHEN NOT MATCHED THEN	
	    INSERT (LocaleId, [Key], TypeName, TextName, Value, UpdatedAt, UpdatedBy)
	    VALUES (source.LocaleId, source.TextKey, source.TypeName, source.TextName, source.Value, source.UpdatedAt, source.UpdatedBy);
";
  /*                  @"MERGE INTO LocalizedTypes 
WHERE [Key] = @TextKey AND LocaleId = @lcid
WHEN matched THEN
    UPDATE SET Value=@value, updat=@updat, updby=@updby
WHEN NOT matched THEN
     INSERT (LocaleId, [Key], TypeName, TextName, Value, UpdatedAt, UpdatedBy)
        VALUES (@lcid, @TextKey, @TypeName, @TextName, @value, @updat, @updby)";
                */
                foreach (var prompt in prompts)
                {
                    
                    //var key = new TypePromptKey(type, name);
                    using (var cmd = _db.Connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        cmd.AddParameter("lcid", prompt.LocaleId);
                        cmd.AddParameter("TextKey", prompt.Key.ToString());
                        cmd.AddParameter("TypeName", prompt.TypeFullName);
                        cmd.AddParameter("TextName", prompt.TextName);
                        cmd.AddParameter("value", prompt.TranslatedText);
                        cmd.AddParameter("updat", DateTime.Now);
                        cmd.AddParameter("updby", Thread.CurrentPrincipal.Identity.Name);
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }

        }

        #endregion

        private void Create(CultureInfo culture, string fullTypeName, string name, string translatedText)
        {
            var sql =
                @"INSERT INTO LocalizedTypes (LocaleId, [Key], TypeName, TextName, Value, UpdatedAt, UpdatedBy)
                      VALUES (@lcid, @TextKey, @TypeName, @TextName, @value, @updat, @updby)";

            var key = new TypePromptKey(fullTypeName, name);
            using (var cmd = _db.Connection.CreateCommand())
            {
                cmd.AddParameter("lcid", culture.LCID);
                cmd.AddParameter("TextKey", key.ToString());
                cmd.AddParameter("TypeName", fullTypeName);
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

        private TypePrompt MapEntity(IDataRecord record)
        {
            // Convert assembly qualified to just full typename
            var fullName = record["TypeName"].ToString();
            int pos = fullName.IndexOf(",");
            if (pos != -1)
                fullName = fullName.Remove(pos);

            return new TypePrompt
                       {
                           LocaleId = (int)record["LocaleId"],
                           TypeFullName = fullName,
                           Key = new TypePromptKey(record["Key"].ToString()),
                           TextName = record["TextName"].ToString(),
                           TranslatedText = record["Value"].ToString(),
                           UpdatedAt = (DateTime)record["UpdatedAt"],
                           UpdatedBy = record["UpdatedBy"].ToString()
                       };
        }
    }
}