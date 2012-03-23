using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.MvcContrib
{
    /// <summary>
    /// Defines the roles used to limit access to the administration features
    /// </summary>
    /// <remarks>You can use this class to change the role names. <c>null</c> = disable role authorization (only recommended during development)</remarks>
    public static class GriffinAdminRoles
    {
        /// <summary>
        /// May access the first page
        /// </summary>
        public static string HomePage = "Admin";

        /// <summary>
        /// May translate views/models/validation messages
        /// </summary>
        public static string Translator = "Translator";

        /// <summary>
        /// May handle account administration
        /// </summary>
        public static string AccountAdmin = "AccountAdmin";

        /// <summary>
        /// Takes a name of a constant and converts it into the containing role name
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <returns>Role name</returns>
        public static string GetRoleFromName(string name)
        {
            switch (name)
            {
                case "HomePage":
                    return HomePage;
                case "Translator":
                    return Translator;
                case "AccountAdmin":
                    return AccountAdmin;
                default:
                    throw new InvalidOperationException("Invalid name: " + name);
            }
        }

        internal const string HomePageName = "HomePage";
        internal const string TranslatorName = "Translator";
        internal const string AccountAdminName = "AccountAdmin";
    }
}
