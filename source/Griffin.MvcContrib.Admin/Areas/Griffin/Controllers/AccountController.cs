using System;
using System.Web.Mvc;
using System.Web.Security;
using Griffin.MvcContrib.Areas.Griffin.Models.Account;
using Griffin.MvcContrib.Providers.Membership;
using MembershipProvider = System.Web.Security.MembershipProvider;

namespace Griffin.MvcContrib.Areas.Griffin.Controllers
{
	public class DumpUser : System.Web.Security.MembershipUser
	{
		
	}

	[Authorize]
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
    		var users = _repository.FindNewAccounts(pageNumber, 50, out totalRecords);
    		return View(new ListModel {Accounts = users, TotalCount = totalRecords});
    	}

		public ActionResult ByEmail(string part, int pageNumber = 1)
		{
			var totalRecords = 0;
			var users = _repository.FindByEmail(part, pageNumber, 50, out totalRecords);
			return View(new SearchModel { Accounts = users, TotalCount = totalRecords });
		}

		public ActionResult ByName(string part, int pageNumber = 1)
		{
			var totalRecords = 0;
			var users = _repository.FindByUserName(part, pageNumber, 50, out totalRecords);
			return View(new SearchModel { Accounts = users, TotalCount = totalRecords });
		}

		public ActionResult List(int pageNumber = 1)
		{
			var totalRecords = 0;
			var users = _repository.FindAll(pageNumber, 50, out totalRecords);
			return View(new ListModel { Accounts = users, TotalCount = totalRecords });
			
		}

		[HttpPost]
		public ActionResult Approve(string id)
		{
			var account = _repository.GetById(id);
			account.IsApproved = true;
			_repository.Update(account);
			return Redirect(Request.UrlReferrer.AbsolutePath);
		}

		public ActionResult Details(string id)
		{
			var account = _repository.GetById(id);

            return View(account);
        }

        public ActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        public ActionResult Create(CreateModel model)
        {
			if (!ModelState.IsValid)
				return View(model);

			try
			{
				MembershipCreateStatus status;
				var user = Membership.CreateUser(model.UserName, model.Password, model.Email, model.PasswordQuestion, model.PasswordAnswer,
				                      model.IsApproved, out status);
				if (status == MembershipCreateStatus.Success)
					return RedirectToAction("Details", new {id = user.ProviderUserKey});

				ModelState.AddModelError("", status.ToString());
			}
            catch (Exception err)
            {
				ModelState.AddModelError("", err.Message);
            }

			return View(model);
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
