using System;
using Griffin.MvcContrib.Providers.Membership;

namespace Griffin.MvcContrib.RavenDb.Providers
{
    /// <summary>
    /// An account have been deleted
    /// </summary>
    public class DeletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeletedEventArgs"/> class.
        /// </summary>
        /// <param name="account">The account.</param>
        public DeletedEventArgs(IMembershipAccount account)
        {
            Account = account;
        }

        /// <summary>
        /// Gets deleted account
        /// </summary>
        public IMembershipAccount Account { get; private set; }
    }
}