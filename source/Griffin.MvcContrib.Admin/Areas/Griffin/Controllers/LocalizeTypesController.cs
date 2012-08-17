using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Mvc;
using Griffin.MvcContrib.Areas.Griffin.Models;
using Griffin.MvcContrib.Areas.Griffin.Models.LocalizeTypes;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using TypePrompt = Griffin.MvcContrib.Localization.Types.TypePrompt;

namespace Griffin.MvcContrib.Areas.Griffin.Controllers
{
    [GriffinAuthorize(GriffinAdminRoles.TranslatorName)]
    [Localized]
    public class LocalizeTypesController : Controller
    {
        private readonly ITypePromptImporter _importer;
        private readonly ILocalizedTypesRepository _repository;

        public LocalizeTypesController(ILocalizedTypesRepository repository)
        {
            _repository = repository;

            // it's optional, since it depends on the implementation
            _importer = DependencyResolver.Current.GetService<ITypePromptImporter>();
            AddValidationPromptsIfMissing();
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            ViewBag.Importer = _importer != null;

            base.OnResultExecuting(filterContext);
        }

        [HttpPost]
        public ActionResult CreateLanguage(string lang)
        {
            try
            {
                _repository.CreateLanguage(new CultureInfo(lang), DefaultUICulture.Value);
                return RedirectToAction("Index", new { lang });
            }
            catch (Exception err)
            {
                ModelState.AddModelError("", err.Message);
                var allPrompts = _repository.GetPrompts(CultureInfo.CurrentUICulture, DefaultUICulture.Value,
                                                        new SearchFilter());
                var model = new IndexModel
                                {
                                    Cultures = _repository.GetAvailableLanguages(),
                                    Prompts = allPrompts.Select(p => new Models.LocalizeTypes.TypePrompt(p))
                                };
                return View("Index", model);
            }
        }

        public ActionResult MakeCommon(string id)
        {
            var key = new TypePromptKey(id);
            var prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, key);
            prompt.TypeFullName = typeof(CommonPrompts).FullName;
            _repository.Save(CultureInfo.CurrentUICulture, typeof(CommonPrompts).FullName, prompt.TextName,
                             prompt.TranslatedText);
            _repository.Delete(CultureInfo.CurrentUICulture, key);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            var key = new TypePromptKey(id);
            _repository.Delete(CultureInfo.CurrentUICulture, key);
            return RedirectToAction("Index");
        }

        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase dataFile)
        {
            var serializer = new DataContractJsonSerializer(typeof(TypePrompt[]));
            var deserialized = serializer.ReadObject(dataFile.InputStream);
            var prompts = (IEnumerable<TypePrompt>)deserialized;
            _importer.Import(prompts);
            return View("Imported", prompts.Count());
        }

        public ActionResult Export()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Export(bool commons, string filter, bool allLanguages)
        {
            var allPrompts = GetPromptsForExport(filter, commons, allLanguages);
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename=type-prompts-{0}.json", DateTime.Now.ToString("yyyyMMdd-HHmm")));
            var serializer = new DataContractJsonSerializer(typeof(List<Localization.Types.TypePrompt>));
            var ms = new MemoryStream();
            serializer.WriteObject(ms, allPrompts);
            ms.Position = 0;
            return File(ms, "application/json");
        }

        public ActionResult ExportPreview(bool commons, string filter, bool allLanguages)
        {
            var allPrompts = GetPromptsForExport(filter, commons, allLanguages);

            var model = allPrompts.Select(x => new Models.LocalizeTypes.TypePrompt(x));
            return PartialView("_ExportPreview", model);
        }

        private List<TypePrompt> GetPromptsForExport(string filter, bool includeCommon, bool allLanguages)
        {
            var cultures = allLanguages
                               ? _repository.GetAvailableLanguages()
                               : new[] { CultureInfo.CurrentUICulture };

            var allPrompts = new List<TypePrompt>();
            foreach (var cultureInfo in cultures)
            {
                var sf = new SearchFilter { Path = filter };
                var prompts = _repository.GetPrompts(cultureInfo, cultureInfo, sf);
                foreach (var prompt in prompts)
                {
                    if (!allPrompts.Any(x => x.LocaleId == prompt.LocaleId && x.Key == prompt.Key))
                        allPrompts.Add(prompt);
                }

                if (includeCommon)
                {
                    sf.Path = typeof(CommonPrompts).Namespace + ".CommonPrompts";
                    prompts = _repository.GetPrompts(cultureInfo, cultureInfo, sf);
                    foreach (var prompt in prompts)
                    {
                        if (!allPrompts.Any(x => x.LocaleId == prompt.LocaleId && x.Key == prompt.Key))
                            allPrompts.Add(prompt);
                    }
                }
            }
            return allPrompts.Where(x => !string.IsNullOrEmpty(x.TranslatedText)).ToList();
        }

        bool OnlyNotTranslated
        {
            get
            {
                var value = Session["OnlyNotTranslated"];
                if (value == null)
                    return false;
                return (bool)value;
            }
            set { Session["OnlyNotTranslated"] = value; }
        }
        string TableFilter
        {
            get
            {
                var value = Session["TableFilter"];
                if (value == null)
                    return null;
                return (string)value;
            }
            set { Session["TableFilter"] = value; }
        }


        public ActionResult Index(FilterModel filter)
        {
            var cookie = Request.Cookies["ShowMetadata"];
            var showMetadata = cookie != null && cookie.Value == "1";
            var languages = _repository.GetAvailableLanguages();

            if (Request.HttpMethod == "POST" && filter != null)
            {
                TableFilter = filter.TableFilter;
                OnlyNotTranslated = filter.OnlyNotTranslated;
            }

            var sf = new SearchFilter();
            if (TableFilter != null)
                sf.Path = TableFilter;
            if (OnlyNotTranslated)
                sf.OnlyNotTranslated = true;

            var prompts =
                _repository.GetPrompts(CultureInfo.CurrentUICulture, DefaultUICulture.Value, sf).Select(
                    p => new Models.LocalizeTypes.TypePrompt(p)).OrderBy(p => p.TypeName).
                    ToList();
            if (!showMetadata)
                prompts = prompts.Where(p => p.TextName == null || !p.TextName.Contains("_")).ToList();


            var model = new IndexModel
                            {
                                Prompts = prompts,
                                Cultures = languages,
                                ShowMetadata = showMetadata,
                                OnlyNotTranslated = OnlyNotTranslated,
                                TableFilter = TableFilter
                            };

            return View(model);
        }

        private void AddValidationPromptsIfMissing()
        {
            if (!DefaultUICulture.IsActive)
                return;

            var prompt = _repository.GetPrompt(DefaultUICulture.Value,
                                               new TypePromptKey(typeof(StringLengthAttribute).FullName, "class"));
            if (prompt == null)
            {
                var provider = new ValidationAttributesStringProvider();
                foreach (var typePrompt in provider.GetPrompts(DefaultUICulture.Value))
                {
                    if (!string.IsNullOrEmpty(typePrompt.TranslatedText))
                        _repository.Save(DefaultUICulture.Value, typePrompt.TypeFullName, typePrompt.TextName,
                                         typePrompt.TranslatedText);
                }
            }
        }

        public ActionResult Edit(string id)
        {
            var model = CreateModel(id);
            return View(model);
        }

        private EditModel CreateModel(string id)
        {
            var key = new TypePromptKey(id);
            var prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, key);
            var defaultLang = _repository.GetPrompt(DefaultUICulture.Value, key);
            var model = new EditModel
                            {
                                DefaultText = defaultLang != null ? defaultLang.TranslatedText : "",
                                LocaleId = prompt.LocaleId,
                                Path =
                                    string.Format("{0} / {1} / {2}", CultureInfo.CurrentUICulture.DisplayName,
                                                  prompt.TypeName, prompt.TextName),
                                Text = prompt.TranslatedText,
                                TextKey = prompt.Key.ToString()
                            };
            return model;
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


            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _repository.Update(CultureInfo.CurrentUICulture, new TypePromptKey(model.TextKey), model.Text);
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                ModelState.AddModelError("", err.Message);
                return View(model);
            }
        }
    }
}