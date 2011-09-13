using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Griffin.MvcContrib.Providers.Membership
{
    public class AccountPasswordInfo
    {
        public AccountPasswordInfo(string username, string password)
        {
            UserName = username;
            Password = password;
        }

        public string PasswordSalt { get; set; }
        public string Password { get; private set; }
        public string UserName { get; private set; }

    }
}