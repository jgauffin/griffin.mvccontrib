using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Griffin.MvcContrib.Areas.Griffin.Models.LocalizeTypes;
using Griffin.MvcContrib.Areas.Griffin.Models.LocalizeViews;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using IndexModel = Griffin.MvcContrib.Areas.Griffin.Models.LocalizeTypes.IndexModel;

namespace Griffin.MvcContrib.Areas.Griffin.Controllers
{
    [Localized]
    public class LocalizeTypesController : System.Web.Mvc.Controller
    {
        private readonly ILocalizedTypesRepository _repository;

        public LocalizeTypesController(ILocalizedTypesRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public ActionResult CreateLanguage(string lang)
        {
            try
            {

                _repository.CreateLanguage(new CultureInfo(lang), new CultureInfo(1033));
                return RedirectToAction("Index", new { lang = lang });
            }
            catch (Exception err)
            {
                ModelState.AddModelError("", err.Message);
                var allPrompts = _repository.GetPrompts(CultureInfo.CurrentUICulture, new CultureInfo(1033), new SearchFilter());
                var model = new IndexModel
                {
                    Cultures = _repository.GetAvailableLanguages(),
                    Prompts = allPrompts.Select(p => new TypePrompt(p))
                };
                return View("Index", model);
            }
        }

        public ActionResult Index()
        {
            var cookie = Request.Cookies["ShowMetadata"];
            var showMetadata = cookie != null && cookie.Value == "1";

            var languges = _repository.GetAvailableLanguages();

            var prompts =
                _repository.GetPrompts(CultureInfo.CurrentUICulture, new CultureInfo(1033), new SearchFilter()).Select(
                    p => new TypePrompt(p)).OrderBy(p => p.TypeName).
                    ToList();
            if (!showMetadata)
                prompts = prompts.Where(p => p.TextName == null || !p.TextName.Contains("_")).ToList();

            var model = new IndexModel
                            {
                                Prompts = prompts,
                                Cultures = languges,
                                ShowMetadata = showMetadata
                            };
            return View(model);
        }

        public ActionResult Edit(string id)
        {
            var key = new TypePromptKey(id);
            var model = _repository.GetPrompt(CultureInfo.CurrentUICulture, key);
            return View(new TypePrompt(model));
        }

        [HttpPost]
        public ActionResult Edit(TypeEditModel model)
        {
            var textPrompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, new TypePromptKey(model.TextKey));
            if (!ModelState.IsValid)
                return View(new TypePrompt(textPrompt));

            try
            {
                _repository.Update(CultureInfo.CurrentUICulture, new TypePromptKey(model.TextKey), model.TranslatedText);
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                ModelState.AddModelError("", err.Message);
                return View(new TypePrompt(textPrompt));
            }
        }
    }
}
