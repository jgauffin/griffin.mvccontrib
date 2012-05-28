using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.MvcContrib.Html.Generators
{
    /// <summary>
    /// Used to create tag builders.
    /// </summary>
    public interface ITagBuilderFactory
    {
        /// <summary>
        /// Create a new tag builder
        /// </summary>
        /// <param name="tagName">Name of html tag, lower case.</param>
        /// <returns>Builder used to generate the tag (if tag has been mapped); otherwise <c>null</c>.</returns>
        ITagBuilder Create(string tagName);

        /// <summary>
        /// Create a new tag builder
        /// </summary>
        /// <param name="tagName">Name of html tag, lower case.</param>
        /// <param name="type">Sub type (for instance "type" attribute in input tags)</param>
        /// <returns>Builder used to generate the tag (if tag+type has been mapped); otherwise <c>null</c>.</returns>
        ITagBuilder Create(string tagName, string type);
    }
}
