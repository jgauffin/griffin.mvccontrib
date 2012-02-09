using System.Web.Mvc;
using Griffin.MvcContrib.Localization;

namespace Localization.Controllers
{
    [Localized]
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }
    }
}