using System.Web.Mvc;
using Griffin.MvcContrib.Localization;

namespace SqlServerLocalization.Controllers
{
    [Localized]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}