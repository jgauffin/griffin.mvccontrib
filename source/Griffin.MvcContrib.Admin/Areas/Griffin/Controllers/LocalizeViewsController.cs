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
				_repository.CreateForLanguage(culture, new CultureInfo(1033));
				return RedirectToAction("Index", new {lang = lang});
			}
			catch(Exception err)
			{
				ModelState.AddModelError("", err.Message);
				var allPrompts = _repository.GetAllPrompts(CultureInfo.CurrentUICulture);
				return View("Index", allPrompts.Select(p => new ViewPrompt(p)));
			}
		}

		public ActionResult Index()
		{
			var allPrompts = _repository.GetAllPrompts(CultureInfo.CurrentUICulture);
			return View(allPrompts.Select(p => new ViewPrompt(p)));
		}

		public ActionResult Edit(string id)
		{
			var prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, id);
			return View(new ViewPrompt(prompt));
		}

		[HttpPost]
		public ActionResult Edit(string textKey, string translatedText)
		{
			var prompt = _repository.GetPrompt(CultureInfo.CurrentUICulture, textKey);
			prompt.TranslatedText = translatedText;
			_repository.Save(prompt);
			return RedirectToAction("Index");
		}
	}
}