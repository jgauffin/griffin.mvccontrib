using System.Web.Mvc;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Models;

namespace Griffin.MvcContrib.Controllers
{
	[Localized]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Message = "Welcome to ASP.NET MVC!";
			var odel = new LogOnModel();
			return View(odel);
		}

		public ActionResult About()
		{
			return View();
		}
	}
}
