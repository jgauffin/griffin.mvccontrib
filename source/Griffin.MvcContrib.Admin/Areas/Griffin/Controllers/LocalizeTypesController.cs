using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Griffin.MvcContrib.Areas.Griffin.Models.LocalizeTypes;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;

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

    	public ActionResult Index()
    	{
    		var cookie = Request.Cookies["ShowMetadata"];
    		var showMetadata = cookie != null && cookie.Value == "1";

    		var languges =_repository.GetAvailableLanguages();

    		var prompts =
    			_repository.GetPrompts(CultureInfo.CurrentUICulture).Select(p => new TypePrompt(p)).OrderBy(p => p.TypeName).
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
