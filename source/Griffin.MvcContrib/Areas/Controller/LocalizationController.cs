using System.Web.Mvc;
using Griffin.MvcContrib.Areas.Models;

namespace Griffin.MvcContrib.Areas.Controller
{
    /// <summary>
    /// Controller used to handle localization
    /// </summary>
    public class LocalizationController : System.Web.Mvc.Controller
    {
        public ActionResult Index()
        {
        	//return Content("Hello wolrd");
        	var model = new ViewTranslationEntry
        	            	{
        	            		TextName = "Hello world"
        	            	};
            return View(model);
        }
    }
}
