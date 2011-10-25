using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Views;
using Griffin.MvcContrib.Admin.Models;

namespace Griffin.MvcContrib.Admin.Controllers
{
	[Localized]
	public class LocalizeViewsController : Controller
	{
		private readonly IViewStringsRepository _repository;

		public LocalizeViewsController(IViewStringsRepository repository)
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