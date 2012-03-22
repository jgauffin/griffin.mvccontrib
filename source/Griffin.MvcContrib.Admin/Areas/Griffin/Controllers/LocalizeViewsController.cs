using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Griffin.MvcContrib.Areas.Griffin.Models.LocalizeViews;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using Griffin.MvcContrib.Localization.Views;
using ViewPrompt = Griffin.MvcContrib.Areas.Griffin.Models.LocalizeViews.ViewPrompt;

namespace Griffin.MvcContrib.Areas.Griffin.Controllers
{
    [Localized]
    public class LocalizeViewsController : Controller
    {
        private readonly IViewLocalizationRepository _repository;
        private IViewPromptImporter _importer;

        public LocalizeViewsController(IViewLocalizationRepository repository)
        {
            _repository = repository;

            // it's optional, since it depends on the implementation
            _importer = DependencyResolver.Current.GetService<IViewPromptImporter>();
        }

        [HttpPost]
        public ActionResult CreateLanguage(string lang)
        {
            try
            {
                var culture = new CultureInfo(lang);
                _repository.CreateLanguage(culture, DefaultUICulture.Value);
                return RedirectToAction("Index", new {lang});
            }
            catch (Exception err)
            {
                ModelState.AddModelError("", err.Message);
                var allPrompts = _repository.GetAllPrompts(CultureInfo.CurrentUICulture, DefaultUICulture.Value,
                                                           new SearchFilter());
                var model = new IndexModel
                                {
                                    Cultures = _repository.GetAvailableLanguages(),
                                    Prompts = allPrompts.Select(p => new ViewPrompt(p))
                                };
                return View("Index", model);
            }
        }

        public ActionResult Index()
        {
            var model = new IndexModel
                            {
                                Cultures = _repository.GetAvailableLanguages(),
                                Prompts =
                                    _repository.GetAllPrompts(CultureInfo.CurrentUICulture, DefaultUICulture.Value,
                                                              new SearchFilter()).Select(p => new ViewPrompt(p))
                            };
            return View(model);
        }

        public ActionResult MakeCommon(string id)
        {
            var key = new ViewPromptKey(id);
            var prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, key);
            prompt.ViewPath = "CommonPrompts";
            _repository.Save(CultureInfo.CurrentUICulture, "CommonPrompts", prompt.TextName, prompt.TranslatedText);
            _repository.Delete(CultureInfo.CurrentUICulture, key);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            var key = new ViewPromptKey(id);
            _repository.Delete(CultureInfo.CurrentUICulture, key);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id)
        {
            var model = CreateModel(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(TranslateModel inmodel)
        {
            var model = CreateModel(inmodel.TextKey);
            model.Text = inmodel.Text;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, new ViewPromptKey(inmodel.TextKey));
                _repository.Save(CultureInfo.CurrentUICulture, prompt.ViewPath, prompt.TextName, inmodel.Text);
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                ModelState.AddModelError("", err.Message);
                return View(model);
            }
        }

        private EditModel CreateModel(string id)
        {
            var key = new ViewPromptKey(id);
            var prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, key);
            var defaultLang = _repository.GetPrompt(DefaultUICulture.Value, key);
            if (prompt == null && defaultLang == null)
                throw new InvalidOperationException("You need to visit the view with the default language first.");

            if (prompt == null)
            {
                _repository.CreatePrompt(CultureInfo.CurrentUICulture, defaultLang.ViewPath, defaultLang.TextName, "");
                prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, key);
            }

            var model = new EditModel
                            {
                                DefaultText =
                                    defaultLang != null && !string.IsNullOrEmpty(defaultLang.TranslatedText)
                                        ? defaultLang.TranslatedText
                                        : prompt.TextName,
                                LocaleId = prompt.LocaleId,
                                Path =
                                    string.Format("{0} / {1}", CultureInfo.CurrentUICulture.DisplayName, prompt.ViewPath),
                                Text = prompt.TranslatedText,
                                TextKey = prompt.Key.ToString()
                            };
            return model;
        }
    }
}