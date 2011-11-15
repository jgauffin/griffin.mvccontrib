using Griffin.MvcContrib.Providers.Roles;

namespace Griffin.MvcContrib.RavenDb.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Role 
    {

        /// <summary>
        /// Gets role name
        /// </summary>
        public string Name { get; set; }


        public string ApplicationName { get; set; }
    }
}
