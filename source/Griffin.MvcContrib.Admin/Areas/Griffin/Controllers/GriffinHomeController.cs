using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Areas.Griffin.Controllers
{
	public class GriffinHomeController : System.Web.Mvc.Controller
    {
        //
        // GET: /Griffin/Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}
