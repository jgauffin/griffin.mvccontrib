using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Griffin.MvcContrib.Areas.Griffin.Models.LocalizeViews;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Views;

namespace Griffin.MvcContrib.Areas.Griffin.Controllers
{
	[Localized]
	public class LocalizeViewsController : System.Web.Mvc.Controller
	{
		private readonly IViewLocalizationRepository _repository;

		public LocalizeViewsController(IViewLocalizationRepository repository)
		{
			_repository = repository;
		}

		[HttpPost]
		public ActionResult CreateLanguage(string lang)
		{
			try
			{
				var culture = new CultureInfo(lang);
				_repository.CreateLanguage(culture, new CultureInfo(1033));
				return RedirectToAction("Index", new {lang = lang});
			}
			catch(Exception err)
			{
				ModelState.AddModelError("", err.Message);
				var allPrompts = _repository.GetAllPrompts(CultureInfo.CurrentUICulture, new CultureInfo(1033), new SearchFilter());
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
		                            _repository.GetAllPrompts(CultureInfo.CurrentUICulture, new CultureInfo(1033),
		                                                      new SearchFilter()).Select(p => new ViewPrompt(p))
		                    };
			return View(model);
		}

		public ActionResult Edit(string id)
		{
			var prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, new ViewPromptKey(id));
			if (prompt == null)
				throw new InvalidOperationException("Failed to find " + id);
			return View(new ViewPrompt(prompt));
		}

		[HttpPost]
		public ActionResult Edit(string textKey, string translatedText)
		{
			var prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, new ViewPromptKey(textKey));
			_repository.Save(CultureInfo.CurrentUICulture, prompt.ViewPath, prompt.TextName, translatedText);
			return RedirectToAction("Index");
		}
	}
}