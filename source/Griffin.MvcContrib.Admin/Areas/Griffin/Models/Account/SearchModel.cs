using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Griffin.MvcContrib.Providers.Membership;

namespace Griffin.MvcContrib.Areas.Griffin.Models.Account
{
	public class SearchModel : ListModel
	{
		public string Part { get; set; }
	}
}