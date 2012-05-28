using System.Web.Mvc;
using System.Web.Routing;

namespace Griffin.MvcContrib.Html.Generators
{
    /// <summary>
    /// Context specific information used when generating tags.
    /// </summary>
    public interface ITagBuilderContext
    {
        /// <summary>
        /// Gets tag name of the root tag
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets full name (if nested model) for the root tag
        /// </summary>
        /// <example>
        /// Department.Title
        /// </example>
        string FullName { get; }

        /// <summary>
        /// Gets meta data for this element.
        /// </summary>
        ModelMetadata Metadata { get; }

        /// <summary>
        /// Gets attributes which should be used for the root tag.
        /// </summary>
        RouteValueDictionary HtmlAttributes { get; }

        /// <summary>
        /// Gets view context
        /// </summary>
        ViewContext ViewContext { get; }
    }
}