using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Localization.Models;

namespace Localization.Controllers
{
    public class HelpersDemoController : Controller
    {
        //
        // GET: /HelpersDemo/

        public ActionResult Index()
        {
            var model = new HelperDemoModel
                            {
                                Age = 10,
                                Ages = new[] {7, 8, 9, 10, 11},
                                InputType = InputType.Password,
                                User = new UserViewModel {FirstName = "Jonas", LastName = "G", Id = 1},
                                Users =
                                    new[]
                                        {
                                            new UserViewModel {FirstName = "Arne", LastName = "Gurra", Id = 2},
                                            new UserViewModel {Id = 1, FirstName = "Jonas", LastName = "G"}
                                        }
                            };
            return View(model);
        }

    }
}
