using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.MvcContrib.Providers.Membership
{
    public static class PasswordExtensions
    {
        public static AccountPasswordInfo CreatePasswordInfo(this IUserAccount account)
        {
            return new AccountPasswordInfo(account.UserName, account.Password) {PasswordSalt = account.PasswordSalt};
        }
    }
}
