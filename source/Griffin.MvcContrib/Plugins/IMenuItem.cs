using System;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Plugins
{
    /// <summary>
    /// A menu item
    /// </summary>
    public interface IMenuItem
    {
        /// <summary>
        /// Gets title to display
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets name (used as HTML id)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Creates the uri which should be visited when the item is clicked
        /// </summary>
        /// <param name="helper">Uri helper (to be able to generate absolute uris)</param>
        /// <returns>Created URI</returns>
        Uri CreateUri(UrlHelper helper);

        /// <summary>
        /// Gets if the item is visible (user have the correct role)
        /// </summary>
        /// <returns>true if user has the correct role; otherwise false.</returns>
        bool IsVisible { get; }
    }
}