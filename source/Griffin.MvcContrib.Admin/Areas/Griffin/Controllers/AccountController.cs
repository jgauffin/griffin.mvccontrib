using System.Web.Mvc;
using System.Web.Security;
using Griffin.MvcContrib.Areas.Griffin.Models.Account;
using Griffin.MvcContrib.Providers.Membership;

namespace Griffin.MvcContrib.Areas.Griffin.Controllers
{
    public class AccountController : System.Web.Mvc.Controller
    {
    	private readonly IAccountRepository _repository;

    	public AccountController(IAccountRepository repository)
		{
			_repository = repository;
		}

    	public ActionResult Index(int pageNumber = 1)
    	{
    		var totalRecords = 0;
    		var users = Membership.GetAllUsers(pageNumber, 50, out totalRecords);
    		return View(new ListModel {Accounts = users, TotalCount = totalRecords});
    	}

		public ActionResult ByEmail(string part, int pageNumber = 1)
		{
			var totalRecords = 0;
			var users = Membership.FindUsersByEmail(part, pageNumber, 50, out totalRecords);
			return View(new SearchModel { Accounts = users, TotalCount = totalRecords });
		}

		public ActionResult ByName(string part, int pageNumber = 1)
		{
			var totalRecords = 0;
			var users = Membership.FindUsersByName(part, pageNumber, 50, out totalRecords);
			return View(new SearchModel { Accounts = users, TotalCount = totalRecords });
		}

		public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
