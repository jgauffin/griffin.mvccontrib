using Griffin.MvcContrib.Localization.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Data.Entity;
using Griffin.MvcContrib.Localization;
using System.Transactions;

namespace Griffin.MvcContrib.EF
{
    public class EFLocalizedViewsRepository : IViewLocalizationRepository, IViewPromptImporter
    {
        ITranslationDbContext _Context;
        IDbSet<LocalizedView> _Set;

        public EFLocalizedViewsRepository(ITranslationDbContext context)
        {
            _Context = context;
            _Set = context.LocalizedViews;
        }

        #region Interface
        public void CreateLanguage(CultureInfo culture, CultureInfo templateCulture)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (templateCulture == null) throw new ArgumentNullException("defaultCulture");

            var allTranslations = _Set.Where(lt => lt.LocaleId == templateCulture.LCID).ToList();
            allTranslations.ForEach(lt =>
            {
                var ltNew = create(lt.ViewPath, lt.TextName, culture, lt.Value);
                _Set.Add(ltNew);
            });
            _Context.Save();
        }

        public void CreatePrompt(CultureInfo culture, string viewPath, string textName, string translatedText)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (viewPath == null) throw new ArgumentNullException("viewPath");
            if (textName == null) throw new ArgumentNullException("textName");
            if (translatedText == null) throw new ArgumentNullException("translatedText");

            var localizedType = create(viewPath, textName, culture, translatedText);
            _Set.Add(localizedType);
            _Context.Save();
        }

        public void Delete(CultureInfo cultureInfo, ViewPromptKey key)
        {
            var lView = getLocalizedView(cultureInfo, key);
            _Set.Remove(lView);
            _Context.Save();
        }

        public bool Exists(CultureInfo cultureInfo)
        {
            return _Set.Any(lv => lv.LocaleId == cultureInfo.LCID);
        }

        public IEnumerable<ViewPrompt> GetAllPrompts(CultureInfo cultureInfo, CultureInfo templateCulture, SearchFilter filter)
        {
            var query = _Set.Where(lt => lt.LocaleId == cultureInfo.LCID);
            if (!string.IsNullOrEmpty(filter.TextFilter))
                query = query.Where(lt => lt.Value.Contains(filter.TextFilter) || lt.TextName.Contains(filter.TextFilter));

            if (!string.IsNullOrEmpty(filter.Path))
                query = query.Where(lt => lt.ViewPath.Contains(filter.Path));
            if (filter.OnlyNotTranslated)
                query = query.Where(lt => lt.Value == null || lt.Value == "");
            var result = query.ToList().Select(lt => lt.ToViewPrompt());
            return result;
        }

        public IEnumerable<CultureInfo> GetAvailableLanguages()
        {
            var cultureIds = _Set.Select(lv => lv.LocaleId).Distinct().ToList();
            return cultureIds.Select(cid => new CultureInfo(cid));
        }

        public ViewPrompt GetPrompt(CultureInfo culture, ViewPromptKey key)
        {
            var type = getLocalizedView(culture, key);
            return type != null ? type.ToViewPrompt() : null;
        }

        public void Save(CultureInfo culture, string viewPath, string textName, string translatedText)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            if (viewPath == null) throw new ArgumentNullException("viewPath");
            if (textName == null) throw new ArgumentNullException("textName");
            if (translatedText == null) throw new ArgumentNullException("translatedText");

            var key = new ViewPromptKey(viewPath, textName);

            // _Context.Save() is done inside "add" and Update "methods"
            if (getLocalizedView(culture, key) == null)
                CreatePrompt(culture, viewPath, textName, translatedText);
            else
                update(culture, key, translatedText);
        }

        public void Import(IEnumerable<ViewPrompt> viewPrompts)
        {
            using (var scope = new TransactionScope())
            {
                viewPrompts.ToList().ForEach(vp => Save(new CultureInfo(vp.LocaleId), vp.ViewPath, vp.TextName, vp.TranslatedText));
                scope.Complete();
            }
        }
        #endregion


        private LocalizedView create(string viewPath, string name, CultureInfo culture, string translatedText)
        {
            var result = new LocalizedView();
            result.ViewPath = viewPath;
            result.TextName = name;
            result.Update(new ViewPromptKey(viewPath, name), translatedText, culture);
            return result;
        }

        private LocalizedView getLocalizedView(CultureInfo culture, ViewPromptKey key)
        {
            var keyS = key.ToString();
            return _Set.Where(vp => vp.LocaleId == culture.LCID && vp.Key == keyS).SingleOrDefault();
        }

        private void update(CultureInfo culture, ViewPromptKey key, string translatedText)
        {
            var lView = getLocalizedView(culture, key);
            lView.Update(key, translatedText, culture);
            _Context.Save();
        }
    }
}
