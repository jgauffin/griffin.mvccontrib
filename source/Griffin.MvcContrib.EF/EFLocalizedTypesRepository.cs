using Griffin.MvcContrib.Localization.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Griffin.MvcContrib.Localization;
using System.Threading;
using System.Data.Entity;
using System.Transactions;

namespace Griffin.MvcContrib.EF
{
    public class EFLocalizedTypesRepository : ILocalizedTypesRepository, ITypePromptImporter
    {
        ITranslationDbContext _Context;
        IDbSet<LocalizedType> _Set;
        public EFLocalizedTypesRepository(ITranslationDbContext context)
        {
            _Context = context;
            _Set = context.LocalizedTypes;
        }

        #region Interface
        public void CreateLanguage(CultureInfo culture, CultureInfo templateCulture)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (templateCulture == null) throw new ArgumentNullException("defaultCulture");

            var allTranslations = _Set.Where(lt => lt.LocaleId == templateCulture.LCID).ToList();
            allTranslations.ForEach(lt =>
            {
                var ltNew = create(lt.TypeName, lt.TextName, culture, lt.Value);
                _Set.Add(ltNew);
            });
            _Context.Save();
        }

        public void Delete(CultureInfo culture, TypePromptKey key)
        {
            var lt = getLocalizedType(culture, key);
            _Set.Remove(lt);
            _Context.Save();
        }

        public IEnumerable<CultureInfo> GetAvailableLanguages()
        {
            var cultureIds = _Set.Select(lt => lt.LocaleId).Distinct().ToList();
            return cultureIds.Select(cid => new CultureInfo(cid));
        }

        public TypePrompt GetPrompt(CultureInfo culture, TypePromptKey key)
        {
            var type = getLocalizedType(culture, key);
            return type != null ? type.ToTypePrompt() : null;
        }

        public IEnumerable<TypePrompt> GetPrompts(CultureInfo cultureInfo, CultureInfo defaultCulture, SearchFilter filter)
        {
            var query = _Set.Where(lt => lt.LocaleId == cultureInfo.LCID);
            if (!string.IsNullOrEmpty(filter.TextFilter))
                query = query.Where(lt => lt.TypeName.Contains(filter.TextFilter) || lt.TextName.Contains(filter.TextFilter));

            if (!string.IsNullOrEmpty(filter.Path))
                query = query.Where(lt => lt.TypeName.Contains(filter.Path));
            if (filter.OnlyNotTranslated)
                query = query.Where(lt => lt.Value == null || lt.Value == "");
            var result = query.ToList().Select(lt => lt.ToTypePrompt());
            return result;
        }

        public void Save(CultureInfo culture, string fullTypeName, string name, string translatedText)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (fullTypeName == null) throw new ArgumentNullException("fullTypeName");
            if (name == null) throw new ArgumentNullException("name");
            if (translatedText == null) throw new ArgumentNullException("translatedText");
            if (fullTypeName.IndexOf(".") == -1)
                throw new ArgumentException("You must use Type.FullName", "fullTypeName");

            var key = new TypePromptKey(fullTypeName, name);

            // _Context.Save() is done inside "add" and Update "methods"
            if (getLocalizedType(culture, key) == null)
                add(culture, fullTypeName, name, translatedText);
            else
                Update(culture, key, translatedText);
        }

        public void Save(CultureInfo culture, Type type, string name, string translatedText)
        {
            Save(culture, type.FullName, name, translatedText);
        }

        public void Update(CultureInfo cultureInfo, TypePromptKey key, string translatedText)
        {
            var localizedType = getLocalizedType(cultureInfo, key);
            localizedType.Update(key, translatedText, cultureInfo);
            _Context.Save();
        }

        public void Import(IEnumerable<TypePrompt> prompts)
        {
            using (var scope = new TransactionScope())
            {
                prompts.ToList().ForEach(pt => Save(new CultureInfo(pt.LocaleId), pt.TypeFullName, pt.TextName, pt.TranslatedText));
                scope.Complete();
            }
        }
        #endregion

        #region Privates methods
        private LocalizedType create(string fullTypeName, string name, CultureInfo culture, string translatedText)
        {
            var result = new LocalizedType();
            result.TypeName = fullTypeName;
            result.TextName = name;
            result.Update(new TypePromptKey(fullTypeName, name), translatedText, culture);
            return result;
        }

        private LocalizedType getLocalizedType(CultureInfo culture, TypePromptKey key)
        {
            return getLocalizedType(culture.LCID, key.ToString());
        }

        private LocalizedType getLocalizedType(int localeID, string key)
        {
            return _Set.Where(tp => tp.LocaleId == localeID && tp.Key == key).SingleOrDefault();
        }

        private void add(CultureInfo culture, string fullTypeName, string name, string translatedText)
        {
            var localizedType = create(fullTypeName, name, culture, translatedText);
            _Set.Add(localizedType);
            _Context.Save();
        }
        #endregion
    }
}
