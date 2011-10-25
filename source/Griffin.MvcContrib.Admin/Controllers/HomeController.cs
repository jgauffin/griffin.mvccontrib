using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Admin.Models;

namespace Griffin.MvcContrib.Admin.Controllers
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
