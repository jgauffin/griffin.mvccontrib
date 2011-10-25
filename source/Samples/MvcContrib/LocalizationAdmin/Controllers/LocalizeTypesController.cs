using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Views;
using LocalizationAdmin.Models;

namespace LocalizationAdmin.Controllers
{
	[Localized]
    public class LocalizeTypesController : Controller
    {
    	private readonly ILocalizedStringRepository _repository;

    	public LocalizeTypesController(ILocalizedStringRepository repository)
        {
        	_repository = repository;
        }

    	public ActionResult Index()
    	{
    		var cookie = Request.Cookies["ShowMetadata"];
    		var showMetadata = cookie != null && cookie.Value == "1";

    		var languges =
    			_repository.GetAvailableLanguages().Select(
    				p =>
    				new SelectListItem
    					{Value = p.Name, Text = p.DisplayName, Selected = p.LCID == CultureInfo.CurrentUICulture.LCID});

    		var prompts = _repository.GetPrompts(CultureInfo.CurrentUICulture).Select(p => new TypePrompt(p)).OrderBy(p => p.TypeName).ToList();
			if (!showMetadata)
				prompts = prompts.Where(p => !p.TextName.Contains("_")).ToList();

    		var model = new ListModel
    		            	{
    		            		Prompts = prompts,
    		            		Languages = languges,
								ShowMetadata = showMetadata
    		            	};
            return View(model);
        }

		public ActionResult Edit(string id)
		{
			var model = _repository.GetPrompt(CultureInfo.CurrentUICulture, id);
			return View(new TypePrompt(model));
		}

		[HttpPost]
		public ActionResult Edit(TypeEditModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			try
			{
				_repository.Update(CultureInfo.CurrentUICulture, model.TextKey, model.TranslatedText);
				return RedirectToAction("Index");
			}
			catch(Exception err)
			{
				ModelState.AddModelError("", err.Message);
				return View(model);
			}
		}



    }
}
