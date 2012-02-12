using System.Collections.Generic;
using Griffin.MvcContrib.Providers.Membership;

namespace Griffin.MvcContrib.Areas.Griffin.Models.Account
{
    public class ListModel
    {
        public IEnumerable<IMembershipAccount> Accounts { get; set; }
        public int TotalCount { get; set; }
    }
}