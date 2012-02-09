using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Griffin.MvcContrib.Localization;

namespace SqlServerLocalization.Controllers
{
    [Localized]
    public class UserController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }

    }
}
