using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Griffin.MvcContrib.Areas.Griffin.Models.Account
{
	public class SearchModel
	{
		public MembershipUserCollection Accounts { get; set; }
		public int TotalCount { get; set; }
		public string Part { get; set; }
	}
}