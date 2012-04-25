using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Griffin.MvcContrib.Plugins
{
    /// <summary>
    ///   Menu item which uses ASP.NET Routes
    /// </summary>
    public class RoutedMenuItem : IMenuWithChildren
    {
        private readonly List<IMenuItem> _children = new List<IMenuItem>();
        private readonly RouteValueDictionary _route;

        /// <summary>
        ///   Initializes a new instance of the <see cref="RoutedMenuItem" /> class.
        /// </summary>
        /// <param name="name"> Name (HTML id). </param>
        /// <param name="title"> Title shown for user. </param>
        /// <param name="route"> The route. </param>
        /// <example>
        ///   <code>mainMenu.Add(new RoutedMenuItem("mnuUsers", "List users", new { controller = "User", action = "Index" });</code>
        /// </example>
        /// <remarks>
        ///   Add the route item "area" for area routes
        /// </remarks>
        public RoutedMenuItem(string name, string title, object route)
        {
            _route = new RouteValueDictionary(route);
            Name = name;
            Title = title;
        }

        #region IMenuWithChildren Members

        /// <summary>
        ///   Gets title to display
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///   Gets name (used as HTML id)
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        ///   Creates the uri which should be visited when the item is clicked
        /// </summary>
        /// <param name="helper"> Uri helper (to be able to generate absolute uris) </param>
        /// <returns> Created URI </returns>
        /// <remarks>
        ///   Add the route item "area" for area routes
        /// </remarks>
        public Uri CreateUri(UrlHelper helper)
        {
            var routeName = (string) _route["area"];
            if (routeName != null)
            {
                _route.Remove("area");
                return new Uri(helper.RouteUrl(routeName, _route), UriKind.Relative);
            }

            return new Uri(helper.RouteUrl(_route), UriKind.Relative);
        }

        /// <summary>
        ///   Gets the enumerator.
        /// </summary>
        /// <returns> </returns>
        public IEnumerator<IMenuItem> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns> An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///   Add a new menu item
        /// </summary>
        /// <param name="menuItem"> Item to add </param>
        public void Add(IMenuItem menuItem)
        {
            _children.Add(menuItem);
        }

        /// <summary>
        ///   Gets the <see cref="IMenuItem" /> with the specified child name.
        /// </summary>
        public IMenuItem this[string childName]
        {
            get { return _children.FirstOrDefault(x => x.Name.Equals(childName, StringComparison.OrdinalIgnoreCase)); }
        }

        /// <summary>
        ///   Checks if a child item exists
        /// </summary>
        /// <param name="childName"> Name of item </param>
        /// <returns> true if found; otherwise false </returns>
        public bool Exists(string childName)
        {
            return this[childName] != null;
        }

        #endregion
    }
}