using System.Linq;
using System.Web.Mvc;
using HtmlHelpersDemo.Models;

namespace HtmlHelpersDemo.Controllers
{
    public class SelectController : Controller
    {
        //
        // GET: /Select/

        public ActionResult Index()
        {
            var model = new ListModel
                            {
                                CurrentUser = Models.User.Users.Last(),
                                Users = Models.User.Users
                            };
            return View(model);
        }
    }
}