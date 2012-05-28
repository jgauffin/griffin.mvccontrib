using System.Collections.Generic;

namespace Griffin.MvcContrib.Plugins
{
    /// <summary>
    /// A menu item which contains children
    /// </summary>
    public interface IMenuWithChildren : IMenuItem, IEnumerable<IMenuItem>
    {
        /// <summary>
        /// Add a new menu item
        /// </summary>
        /// <param name="menuItem">Item to add</param>
        void Add(IMenuItem menuItem);

        /// <summary>
        /// Gets child menu item
        /// </summary>
        /// <param name="childName">Name of item</param>
        /// <returns>Item if found; otherwise null</returns>
        IMenuItem this[string childName] { get; }

        /// <summary>
        /// Checks if a child item exists
        /// </summary>
        /// <param name="childName">Name of item</param>
        /// <returns>true if found; otherwise false</returns>
        bool Exists(string childName);
    }
}