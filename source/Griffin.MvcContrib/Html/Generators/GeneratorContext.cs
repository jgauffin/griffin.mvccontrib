using System.Web.Mvc;
using System.Web.Routing;

namespace Griffin.MvcContrib.Html.Generators
{
    /// <summary>
    /// Context used when generating new elements.
    /// </summary>
    public class GeneratorContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorContext"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="fullName">The full name (dot notation). If it's a complex/nested model.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        public GeneratorContext(string name, string fullName, ModelMetadata metadata,
                                RouteValueDictionary htmlAttributes)
        {
            Name = name;
            FullName = fullName;
            Metadata = metadata;
            HtmlAttributes = htmlAttributes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorContext"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected GeneratorContext(GeneratorContext context)
        {
            Name = context.Name;
            FullName = context.FullName;
            Metadata = context.Metadata;
            HtmlAttributes = new RouteValueDictionary(context.HtmlAttributes);
        }

        /// <summary>
        /// Gets input name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets full name (if nested model)
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Gets meta data for this element.
        /// </summary>
        public ModelMetadata Metadata { get; private set; }

        /// <summary>
        /// Gets attributes specified with the helper.
        /// </summary>
        public RouteValueDictionary HtmlAttributes { get; private set; }
    }
}